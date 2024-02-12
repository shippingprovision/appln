using System;
using System.Collections.Generic;

using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using NHibernate.Transaction;
using System.Reflection;
using System.Collections;

namespace ShippingProvision.Services
{
    public abstract class NHibernateManagerBase<TObject>
        where TObject: class
    {
        #region Constants

        private const Int32 DEFAULT_MAX_RESULTS = 100;

        #endregion

        #region Fields

        private ISession _session;

        #endregion

        #region Properties

        public System.Type Type
        {
            get { return typeof(TObject); }
        }

        public ISession Session
        {
            get { return this._session; }
            protected set { this._session = value; }
        }

        public IQueryable<TObject> Items
        {
            get { return this.Session.Query<TObject>(); }
        }

        #endregion

        #region Operations

        #region Get Operatons

        public virtual TObject GetById(long id)
        {
            return (TObject)this.Session.Get<TObject>(id);
        }

        public virtual IList<TObject> GetAll()
        {
            return this.GetAll(DEFAULT_MAX_RESULTS);
        }

        public virtual IList<TObject> GetAll(int maxResults)
        {
            return this.GetByCriteria(maxResults);
        }

        public virtual IList<TObject> GetByCriteria(params ICriterion[] criterionList)
        {
            return this.GetByCriteria(DEFAULT_MAX_RESULTS, criterionList);
        }

        public virtual IList<TObject> GetByCriteria(int maxResults, params ICriterion[] criterionList)
        {
            ICriteria criteria = Session.CreateCriteria<TObject>().SetMaxResults(maxResults);

            foreach (ICriterion criterion in criterionList)
            {
                criteria.Add(criterion);
            }

            return (IList<TObject>)(criteria.List<TObject>());
        }

        public virtual IList<TObject> GetByExample(TObject exampleObject, params string[] excludePropertyList)
        {
            return this.GetByExample(DEFAULT_MAX_RESULTS, exampleObject, excludePropertyList);
        }

        public virtual IList<TObject> GetByExample(int maxResults, TObject exampleObject, params string[] excludePropertyList)
        {
            ICriteria criteria = Session.CreateCriteria<TObject>().SetMaxResults(maxResults);
            Example example = Example.Create(exampleObject);

            foreach (string excludeProperty in excludePropertyList)
            {
                example.ExcludeProperty(excludeProperty);
            }

            criteria.Add(example);

            return (IList<TObject>)(criteria.List<TObject>());
        }

        public virtual TObject GetObjectForUpdate(long id, int rev)
        {
            var criteria = this.Session
                               .CreateCriteria<TObject>()
                               .Add(Expression.Eq("Id", id))
                               .Add(Expression.Eq("Rev", rev));

            IList list = criteria.List();
            var objectForUpdate = (list != null && list.Count > 0) ? (TObject)list[0] : null;
            if (objectForUpdate == null)
            {
                throw new Exception("Object seems to be updated by someone else.");
            }
            return objectForUpdate;
        }

        #endregion

        #region CRUD Operations

        public virtual TObject Save(TObject entity)
        {
            TObject obj = default(TObject);
            if (entity != null)
            {
                MarkUpdate(entity);
                obj = this.Session.Save(entity) as TObject;         
            }
            return obj;
        }

        public virtual void SaveOrUpdate(TObject entity)
        {
            if (entity != null)
            {
                MarkUpdate(entity);
                this.Session.SaveOrUpdate(entity);
            }
        }

        public virtual void MarkAsDelete(TObject entity)
        {
            if (entity != null)
            {
                MarkDelete(entity);
                this.Session.SaveOrUpdate(entity);
            }
        }

        public virtual void Delete(TObject entity)
        {
            if (entity != null)
            {
                this.Session.Delete(entity);
            }
        }

        public virtual void Update(TObject entity)
        {
            if (entity != null)
            {
                MarkUpdate(entity);
                this.Session.Update(entity);
            }
        }

        public virtual void Refresh(TObject entity)
        {
            if (entity != null)
            {
                this.Session.Refresh(entity);
            }
        }

        private void MarkUpdate(TObject entity)
        {
            var obj = entity as Entity<long>;
            if (obj != null)
            {
                obj.Status = (int)Enums.Status.Alive;
            }
            var his = entity as HistoryEntity<long>;
            if (his != null)
            {
                if (his.Id == 0)
                {
                    his.CreatedBy = GetUserId();
                    his.CreatedDate = DateTime.Now;
                }
                his.ModifiedBy = GetUserId();
                his.ModifiedDate = DateTime.Now;
            }
        }

        private void MarkDelete(TObject entity)
        {
            var obj = entity as Entity<long>;
            if (obj != null)
            {
                obj.Status = (int)Enums.Status.Deleted;
            }
            var his = entity as HistoryEntity<long>;
            if (his != null)
            {
                his.ModifiedBy = GetUserId();
                his.ModifiedDate = DateTime.Now;
            }
        }

        protected virtual long GetUserId()
        {
            return 0;
        }
        
        #endregion

        protected IDbTransaction GetIDbTransaction(ITransaction hibernateTx)
        {
            AdoTransaction hibernateAdoTx = hibernateTx as AdoTransaction;
            IDbTransaction adoTransaction = null;
            if (hibernateAdoTx != null)
            {
                try
                {
                    FieldInfo fi = hibernateAdoTx.GetType().GetField("trans", BindingFlags.Instance | BindingFlags.NonPublic);
                    adoTransaction = fi.GetValue(hibernateAdoTx) as IDbTransaction;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return adoTransaction;
        }

        public List<Dictionary<string, object>> ExecuteSQLQuery(string sqlText, Dictionary<String, object> parameters)
        {
            var returnval = new List<Dictionary<string, object>>();

            IDbCommand command = new SqlCommand();
            command.Connection = Session.Connection;
            command.CommandType = CommandType.Text;
            command.CommandText = sqlText;
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(new SqlParameter(parameter.Key,parameter.Value));
            }
            command.Transaction = GetIDbTransaction(Session.Transaction);

            // Execute the stored procedure
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                int count = reader.FieldCount;
                for (int i = 0; i < count; i++)
                {
                    if (!reader.GetName(i).Equals(""))
                        dict.Add(reader.GetName(i), reader.GetValue(i));
                }
                returnval.Add(dict);
            }
            if (reader != null && !reader.IsClosed)
            {
                reader.Close();
            }
            return returnval;
        }

        public List<Dictionary<string, object>> ExecuteStoredProcedure(string name, Dictionary<String, object> parameters)
        {
            var returnval = new List<Dictionary<string, object>>();

            IDbCommand command = new SqlCommand();
            command.Connection = Session.Connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(new SqlParameter(parameter.Key,parameter.Value));
            }
            command.Transaction = GetIDbTransaction(Session.Transaction);

            // Execute the stored procedure
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                int count = reader.FieldCount;
                for (int i = 0; i < count; i++)
                {
                    if (!reader.GetName(i).Equals(""))
                        dict.Add(reader.GetName(i), reader.GetValue(i));
                }
                returnval.Add(dict);
            }
            if (reader != null && !reader.IsClosed)
            {
                reader.Close();
            }
            return returnval;
        }

        #endregion

        #region Constructors

        public NHibernateManagerBase()
        {
            this.Session = NHibernateSessionManager.Instance.RetrieveSessionFrom("DB");
        }


        public NHibernateManagerBase(string sessionFactoryConfigPath)
        {
            this.Session = NHibernateSessionManager.Instance.RetrieveSessionFrom(sessionFactoryConfigPath);
        }

        public NHibernateManagerBase(ISession session)
        {
            this.Session = session;
        }

        #endregion
    }
}
