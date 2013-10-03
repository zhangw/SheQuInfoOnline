using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace Wedo.Utility.Reflection.HistoryLog
{
	/// <summary>
	/// 变更记录操作类
	/// </summary>
	public interface IHistoryLog<T> where T : class
	{
		/// <summary>
		/// 创建变更
		/// </summary>
		///<param name="objectVal">变更后的对象</param>
		///<param name="sourceVal">源对象</param>
		void Create(T sourceVal, T objectVal, string createdBy, string ownerId);

		/// <summary>
		/// 获取被变更记录的所有变更历史
		/// </summary>
		/// <param name="ownerId">被变更记录编号</param>        
		/// <returns></returns>
		List<HistoryLogModel> GetHistories(string ownerId);

		/// <summary>
		/// 获取最后的变更记录
		/// </summary>
		/// <param name="owerId">被变更记录编号</param>
		/// <returns></returns>
		HistoryLogModel GetLastHistory(string owerId);
	}

	/// <summary>
	/// 基础的实现类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BaseHistoryLog<T> : IHistoryLog<T> where T : class
	{
		/// <summary>
		/// 获取最新版本号
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		private int NewVersion(string ownerId)
		{
			var item = GetLastHistory(ownerId);
			int version = 1;
			if (item != null)
				version = item.Version + 1;
			return version;
		}

		/// <summary>
		/// 创建实体
		/// </summary>
		/// <param name="sourceVal"></param>
		/// <param name="objectVal"></param>
		/// <param name="createdBy"></param>
		/// <param name="ownerId"></param>
		public void Create(T sourceVal, T objectVal, string createdBy, string ownerId)
		{
			HistorylHelper<T> helper = new HistorylHelper<T>(createdBy, ownerId, NewVersion(ownerId));
			var model = helper.CreateHistory(sourceVal, objectVal);
			if (model != null)
				Save(model);
		}

		/// <summary>
		/// 保存到数据库或其他持久化设备
		/// </summary>
		/// <param name="model"></param>
		protected abstract void Save(HistoryLogModel model);

		public abstract List<HistoryLogModel> GetHistories(string ownerId);

		public abstract HistoryLogModel GetLastHistory(string owerId);
	}

	/// <summary>
	/// SQL实现方式
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SqlHsitoryLog<T> : BaseHistoryLog<T> where T : class
	{
		private string _ConnString;

		public SqlHsitoryLog(string connString)
		{
			if (string.IsNullOrEmpty(_ConnString))
				_ConnString = connString;
		}

		protected override void Save(HistoryLogModel model)
		{
			#region main table
			SqlCommand cmd = new SqlCommand(@"INSERT INTO HistoryLogModel(Id,ModelName,OwnerId,Version,ChangeMan,CreatedBy,CreatedTime)VALUES
                                             (@Id,@ModelName,@OwnerId,@Version,@ChangeMan,@CreatedBy,@CreatedTime)");
			cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Id", Value = model.Id });
			cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ModelName", Value = model.ModelName });
			cmd.Parameters.Add(new SqlParameter() { ParameterName = "@OwnerId", Value = model.OwnerId });
			cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Version", Value = model.Version });

			cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ChangeMan", Value = model.ChangeMan });
			cmd.Parameters.Add(new SqlParameter() { ParameterName = "@CreatedBy", Value = model.CreatedBy });
			cmd.Parameters.Add(new SqlParameter() { ParameterName = "@CreatedTime", Value = DateTime.Now });
			#endregion

			#region child table
			SqlCommand childCmd = new SqlCommand(@"INSERT INTO HistoryLogItem(Id,ParentId,Name,Title,NewValue,SourceValue)VALUES(@Id,@ParentId,@Name,@Title,@NewValue,@SourceValue)");
			childCmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier);
			childCmd.Parameters.Add("@ParentId", SqlDbType.UniqueIdentifier);
			childCmd.Parameters.Add("@Name", SqlDbType.VarChar);

			childCmd.Parameters.Add("@Title", SqlDbType.VarChar);
			childCmd.Parameters.Add("@NewValue", SqlDbType.VarChar);
			childCmd.Parameters.Add("@SourceValue", SqlDbType.VarChar);
			#endregion

			using (SqlConnection conn = new SqlConnection(_ConnString))
			{
				if (conn.State != ConnectionState.Open)
					conn.Open();
				cmd.Connection = conn;
				cmd.ExecuteNonQuery();

				foreach (var item in model.ModelChangeLogItems)
				{
					childCmd.Connection = conn;
					childCmd.Parameters["@Id"].Value = item.Id;
					childCmd.Parameters["@ParentId"].Value = item.ParentId;
					childCmd.Parameters["@Name"].Value = item.Name;

					childCmd.Parameters["@Title"].Value = item.Title;
					childCmd.Parameters["@NewValue"].Value = item.NewValue;
					childCmd.Parameters["@SourceValue"].Value = item.SourceValue;
					childCmd.ExecuteNonQuery();
				}
			}
		}

		public override List<HistoryLogModel> GetHistories(string ownerId)
		{
			List<HistoryLogModel> list = new List<HistoryLogModel>();
			using (SqlConnection conn = new SqlConnection(_ConnString))
			{
				if (conn.State != System.Data.ConnectionState.Open)
					conn.Open();
				SqlCommand cmd = new SqlCommand("SELECT * FROM HistoryLogModel WHERE OwnerId=@OwnerId AND ModelName=@ModelName", conn);
				cmd.Parameters.Add("@OwnerId", SqlDbType.VarChar);
				cmd.Parameters.Add("@ModelName", SqlDbType.VarChar);

				cmd.Parameters["@OwnerId"].Value = ownerId;
				cmd.Parameters["@ModelName"].Value = typeof(T).Name;

				using (var reader = cmd.ExecuteReader())
				{
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							list.Add(GetFromRdr(reader));
						}
					}
				}

				foreach (var item in list)
				{
					item.ModelChangeLogItems = GetItems(conn, item.Id);
				}
			}
			return list;
		}

		public override HistoryLogModel GetLastHistory(string ownerId)
		{
			HistoryLogModel model = null;
			using (SqlConnection conn = new SqlConnection(_ConnString))
			{
				if (conn.State != System.Data.ConnectionState.Open)
					conn.Open();
				SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM HistoryLogModel WHERE OwnerId=@OwnerId AND ModelName=@ModelName ORDER BY CreatedTime DESC", conn);
				cmd.Parameters.Add("@OwnerId", SqlDbType.VarChar);
				cmd.Parameters.Add("@ModelName", SqlDbType.VarChar);
				cmd.Parameters["@OwnerId"].Value = ownerId;
				cmd.Parameters["@ModelName"].Value = typeof(T).Name;

				using (var reader = cmd.ExecuteReader())
				{
					if (reader.HasRows && reader.Read())
					{
						model = GetFromRdr(reader);
					}
				}
				if (model != null)
					model.ModelChangeLogItems = GetItems(conn, model.Id);
			}
			return model;
		}

		private List<HistoryLogItem> GetItems(SqlConnection conn, Guid parentId)
		{
			var items = new List<HistoryLogItem>();
			SqlCommand cmd = new SqlCommand("SELECT * FROM HistoryLogItem WHERE ParentId=@ParentId", conn);
			cmd.Parameters.Add(new SqlParameter()
			{
				ParameterName = "ParentId",
				Value = parentId
			});
			using (SqlDataReader reader = cmd.ExecuteReader())
			{
				while (reader.Read())
					items.Add(GetHistoryItem(reader, parentId));
			}
			return items;
		}

		private HistoryLogItem GetHistoryItem(SqlDataReader reader, Guid parentId)
		{
			HistoryLogItem item = new HistoryLogItem();

			item.Id = (Guid)reader["Id"];
			item.Name = reader["Name"].ToString();
			item.Title = reader["Title"].ToString();
			item.NewValue = reader["NewValue"].ToString();
			item.SourceValue = reader["SourceValue"].ToString();
			item.ParentId = parentId;

			return item;
		}

		private HistoryLogModel GetFromRdr(SqlDataReader reader)
		{
			HistoryLogModel model = new HistoryLogModel();
			model.Id = (Guid)reader["Id"];
			model.ModelName = reader["ModelName"].ToString();
			model.OwnerId = reader["OwnerId"].ToString();
			model.Version = Convert.ToInt32(reader["Version"]);
			model.ChangeMan = reader["ChangeMan"].ToString();
			model.CreatedBy = reader["CreatedBy"].ToString();
			model.CreatedTime = Convert.ToDateTime(reader["CreatedTime"]);
			return model;
		}
	}

	/// <summary>
	/// 变更记录操作类工厂类,如果要使用SQL的方式，则必须在配置文件的连接字符串中添加一个HistoryConn项
	/// </summary>
	public static class HistoryLogFactory
	{
		private static object CreateGeneric(Type generic, Type innerType, params object[] args)
		{
			System.Type specificType = generic.MakeGenericType(new System.Type[] { innerType });
			return Activator.CreateInstance(specificType, args);
		}

		/// <summary>
		/// 创建History类的实例
		/// </summary>
		/// <typeparam name="T">具体类型</typeparam>
		/// <returns></returns>
		public static IHistoryLog<T> Create<T>() where T : class
		{
			Assembly asm = Assembly.Load("Wedo.Utility");
			Type ty = asm.GetType("Wedo.Utility.Reflection.HistoryLog.SqlHsitoryLog`1");
			string conn = System.Configuration.ConfigurationManager.ConnectionStrings["HistoryConn"].ConnectionString;//"Application Name=TE_Application;Data source=(local);database=Test;user id=sa;password=p@ssw0rd;";
			return (IHistoryLog<T>)CreateGeneric(ty, typeof(T), conn);
		}
	}
}
