namespace Avanade.BootStrapper.Web.Task
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TaskPriorityAttribute : Attribute
    {
        #region Constructors

        public TaskPriorityAttribute()
        {
        }

        public TaskPriorityAttribute(int priority)
        {
            Priority = priority;
        }

        #endregion Constructors

        #region Properties

        public int Priority
        {
            get; set;
        }

        #endregion Properties
    }
}