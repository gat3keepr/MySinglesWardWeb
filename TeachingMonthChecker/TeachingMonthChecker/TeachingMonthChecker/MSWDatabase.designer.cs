﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TeachingMonthChecker
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="MSW")]
	public partial class MSWDatabase : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InserttTeachingMonth(tTeachingMonth instance);
    partial void UpdatetTeachingMonth(tTeachingMonth instance);
    partial void DeletetTeachingMonth(tTeachingMonth instance);
    #endregion
		
		public MSWDatabase() : 
				base(global::TeachingMonthChecker.Properties.Settings.Default.MSWConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public MSWDatabase(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MSWDatabase(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MSWDatabase(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MSWDatabase(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<tTeachingMonth> tTeachingMonths
		{
			get
			{
				return this.GetTable<tTeachingMonth>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.tTeachingMonth")]
	public partial class tTeachingMonth : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _TeachingMonthID;
		
		private System.DateTime _teachingMonth;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnTeachingMonthIDChanging(int value);
    partial void OnTeachingMonthIDChanged();
    partial void OnteachingMonthChanging(System.DateTime value);
    partial void OnteachingMonthChanged();
    #endregion
		
		public tTeachingMonth()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TeachingMonthID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int TeachingMonthID
		{
			get
			{
				return this._TeachingMonthID;
			}
			set
			{
				if ((this._TeachingMonthID != value))
				{
					this.OnTeachingMonthIDChanging(value);
					this.SendPropertyChanging();
					this._TeachingMonthID = value;
					this.SendPropertyChanged("TeachingMonthID");
					this.OnTeachingMonthIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_teachingMonth", DbType="Date NOT NULL")]
		public System.DateTime teachingMonth
		{
			get
			{
				return this._teachingMonth;
			}
			set
			{
				if ((this._teachingMonth != value))
				{
					this.OnteachingMonthChanging(value);
					this.SendPropertyChanging();
					this._teachingMonth = value;
					this.SendPropertyChanged("teachingMonth");
					this.OnteachingMonthChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591