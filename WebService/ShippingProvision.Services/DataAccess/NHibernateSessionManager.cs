using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Web;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg;

namespace ShippingProvision.Services
{
    /// <summary>
    /// Handles creation and management of sessions and transactions.  It is a singleton because 
    /// building the initial session factory is very expensive. 
    /// </summary>
    /// <remarks>
    /// Refer Chapter 8 of "Hibernate in Action" by Bauer and King.  
    /// </remarks>
    public sealed class NHibernateSessionManager
    {
        #region Constants

        private const string TRANSACTION_KEY = "CONTEXT_TRANSACTIONS";
        private const string SESSION_KEY = "CONTEXT_SESSIONS";

        #endregion

        #region Fields

        private Hashtable _sessionFactories = new Hashtable();

        #endregion

        #region Properties

        public Func<ISessionFactory> SessionFactoryProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Checks if NHibernate is running within a web context
        /// </summary>
        /// <returns></returns>
        private bool IsInWebContext
        {
            get { return HttpContext.Current != null; }
        }

        /// <summary>
        /// Since multiple databases may be in use, there may be one transaction per database 
        /// persisted at any one time.  The easiest way to store them is via a hashtable
        /// with the key being tied to session factory.  If within a web context, this uses
        /// "HttpContext" instead of the WinForms specific "CallContext".  
        /// </summary>
        /// <remarks>
        /// Discussion concerning this found at http://forum.springframework.net/showthread.php?t=572
        /// </remarks>
        private Hashtable ContextTransactions
        {
            get
            {
                if (IsInWebContext)
                {
                    if (HttpContext.Current.Items[TRANSACTION_KEY] == null)
                    {
                        HttpContext.Current.Items[TRANSACTION_KEY] = new Hashtable();
                    }

                    return (Hashtable)HttpContext.Current.Items[TRANSACTION_KEY];
                }
                else
                {
                    if (CallContext.GetData(TRANSACTION_KEY) == null)
                    {
                        CallContext.SetData(TRANSACTION_KEY, new Hashtable());
                    }

                    return (Hashtable)CallContext.GetData(TRANSACTION_KEY);
                }
            }
        }

        /// <summary>
        /// Since multiple databases may be in use, there may be one session per database 
        /// persisted at any one time.  The easiest way to store them is via a hashtable
        /// with the key being tied to session factory.  If within a web context, this uses
        /// "HttpContext" instead of the WinForms specific.  
        /// </summary>
        /// <remarks>
        /// Discussion concerning this found at http://forum.springframework.net/showthread.php?t=572
        /// </remarks>
        private Hashtable ContextSessions
        {
            get
            {
                if (IsInWebContext)
                {
                    if (HttpContext.Current.Items[SESSION_KEY] == null)
                        HttpContext.Current.Items[SESSION_KEY] = new Hashtable();

                    return (Hashtable)HttpContext.Current.Items[SESSION_KEY];
                }
                else
                {
                    if (CallContext.GetData(SESSION_KEY) == null)
                        CallContext.SetData(SESSION_KEY, new Hashtable());

                    return (Hashtable)CallContext.GetData(SESSION_KEY);
                }
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// This is a thread-safe, lazy singleton.  
        /// </summary>
        /// <remarks>
        /// See http://www.yoda.arachsys.com/csharp/singleton.html for more details about its implementation.
        /// </remarks>
        public static NHibernateSessionManager Instance
        {
            get { return Nested.NHibernateSessionManager; }
        }

        /// <summary>
        /// Allows you to register an interceptor on a new session.  This may not be called if there is already
        /// an open session attached to the HttpContext.  If you have an interceptor to be used, modify
        /// the HttpModule to call this before calling BeginTransaction().
        /// </summary>
        /// <param name="sessionFactoryConfigPath"></param>
        /// <param name="interceptor"></param>
        public void RegisterInterceptorOn(string sessionFactoryConfigPath, IInterceptor interceptor)
        {
            ISession session = (ISession)ContextSessions[sessionFactoryConfigPath];

            if (session != null && session.IsOpen)
            {
                throw new CacheException("You cannot register an interceptor once a session has already been opened");
            }

            RetrieveSessionFrom(sessionFactoryConfigPath, interceptor);
        }

        /// <summary>
        /// Get session for configured factory
        /// </summary>
        /// <param name="sessionFactoryConfigPath"></param>
        /// <returns></returns>
        public ISession RetrieveSessionFrom(string sessionFactoryConfigPath)
        {
            return RetrieveSessionFrom(sessionFactoryConfigPath, null);
        }

        /// <summary>
        /// Flushes anything left in the session and closes the connection.
        /// </summary>
        /// <param name="sessionFactoryConfigPath"
        public void CloseSessionOn(string sessionFactoryConfigPath)
        {
            ISession session = (ISession)ContextSessions[sessionFactoryConfigPath];

            if (session != null && session.IsOpen)
            {
                session.Flush();
                session.Close();
            }

            ContextSessions.Remove(sessionFactoryConfigPath);
        }

        /// <summary>
        /// Flushes anything left in the session.
        /// </summary>
        /// <param name="sessionFactoryConfigPath"
        public void FlushSessionOn(string sessionFactoryConfigPath)
        {
            ISession session = (ISession)ContextSessions[sessionFactoryConfigPath];

            if (session != null && session.IsOpen)
            {
                session.Flush();
            }
        }

        /// <summary>
        /// Clear all objects in the session.
        /// </summary>
        /// <param name="sessionFactoryConfigPath"
        public void ClearSessionOn(string sessionFactoryConfigPath)
        {
            ISession session = (ISession)ContextSessions[sessionFactoryConfigPath];

            if (session != null && session.IsOpen)
            {
                session.Clear();
            }
        }

        /// <summary>
        /// Start a transaction on session
        /// </summary>
        /// <param name="sessionFactoryConfigPath"></param>
        /// <returns></returns>
        public ITransaction BeginTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryConfigPath];

            if (transaction == null)
            {
                transaction = RetrieveSessionFrom(sessionFactoryConfigPath).BeginTransaction();
                ContextTransactions.Add(sessionFactoryConfigPath, transaction);
            }

            return transaction;
        }

        /// <summary>
        /// Commit transaction on a session
        /// </summary>
        /// <param name="sessionFactoryConfigPath"></param>
        public void CommitTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryConfigPath];

            try
            {
                if (HasOpenTransactionOn(sessionFactoryConfigPath))
                {
                    transaction.Commit();
                    ContextTransactions.Remove(sessionFactoryConfigPath);
                }
            }
            catch (HibernateException)
            {
                RollbackTransactionOn(sessionFactoryConfigPath);
                throw;
            }
        }

        /// <summary>
        /// Check is transaction for session already open
        /// </summary>
        /// <param name="sessionFactoryConfigPath"></param>
        /// <returns></returns>
        public bool HasOpenTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryConfigPath];

            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        /// <summary>
        /// Rollback transactions on a session
        /// </summary>
        /// <param name="sessionFactoryConfigPath"></param>
        public void RollbackTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryConfigPath];

            try
            {
                if (HasOpenTransactionOn(sessionFactoryConfigPath))
                {
                    transaction.Rollback();
                }

                ContextTransactions.Remove(sessionFactoryConfigPath);
            }
            finally
            {
                //CloseSessionOn(sessionFactoryConfigPath);
            }
        }

        /// <summary>
        /// This method attempts to find a session factory stored in "sessionFactories"
        /// via its name; if it can't be found it creates a new one and adds it the hashtable.
        /// </summary>
        /// <param name="sessionFactoryConfigPath">Path location of the factory config</param>
        private ISessionFactory RetrieveSessionFactoryFor(string sessionFactoryConfigPath)
        {
            try
            {
                //  Attempt to retrieve a stored SessionFactory from the hashtable.
                ISessionFactory sessionFactory = (ISessionFactory)_sessionFactories[sessionFactoryConfigPath];

                //  Failed to find a matching SessionFactory so make a new one.
                if (sessionFactory == null)
                {
                    if (SessionFactoryProvider != null)
                    {
                        sessionFactory = SessionFactoryProvider();
                    }
                    if (sessionFactory == null)
                    {
                        Configuration cfg = new Configuration();
                        cfg.Configure(sessionFactoryConfigPath);

                        //  Now that we have our Configuration object, create a new SessionFactory
                        sessionFactory = cfg.BuildSessionFactory();
                    }
                    if (sessionFactory == null)
                    {
                        throw new InvalidOperationException("cfg.BuildSessionFactory() returned null.");
                    }

                    _sessionFactories.Add(sessionFactoryConfigPath, sessionFactory);
                }

                return sessionFactory;
            }
            catch (Exception ex)
            {
                throw new Exception("Retrieve Session Factory Failed", ex);
            }
        }

        /// <summary>
        /// Gets a session with or without an interceptor.  This method is not called directly; instead,
        /// it gets invoked from other public methods.
        /// </summary>
        private ISession RetrieveSessionFrom(string sessionFactoryConfigPath, IInterceptor interceptor)
        {
            ISession session = (ISession)ContextSessions[sessionFactoryConfigPath];

            if (session == null)
            {
                if (interceptor != null)
                {
                    session = RetrieveSessionFactoryFor(sessionFactoryConfigPath).OpenSession(interceptor);
                }
                else
                {
                    session = RetrieveSessionFactoryFor(sessionFactoryConfigPath).OpenSession();
                }

                ContextSessions[sessionFactoryConfigPath] = session;
            }

            return session;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>
        private NHibernateSessionManager() { }

        #endregion

        #region Classes

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }

            internal static readonly NHibernateSessionManager NHibernateSessionManager = new NHibernateSessionManager();
        }

        #endregion
    }
}
