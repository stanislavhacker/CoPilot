using CoPilot.Core.Data;
using CoPilot.Core.Utils;
using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoPilot.CoPilot.Controller
{
    public class Scheduler : Base
    {
        #region PRIVATE

        private Data dataController;

        #endregion

        /// <summary>
        /// Scheduler 
        /// </summary>
        /// <param name="dataController"></param>
        public Scheduler(Data dataController)
        {
            //data controller
            this.dataController = dataController;

            //update
            this.Update();
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            var maintenances = this.dataController.Maintenances;

            foreach (var maintenance in maintenances)
            {
                if (maintenance.IsOdometer)
                {
                    this.showWarning(maintenance);
                }
                else
                {
                    this.createReminder(maintenance);
                }
            }
        }

        /// <summary>
        /// Show warning
        /// </summary>
        /// <param name="maintenance"></param>
        private void showWarning(Maintenance maintenance)
        {
            var repairs = this.dataController.Repairs;
            var refill = this.dataController.Fills;
            var odometer = 0.0;
            var maintenanceOdometer = 0.0;

            if (repairs.Count > 0) {
                odometer = DistanceExchange.GetOdometerWithRightDistance(repairs[0].Odometer);
            }
            if (refill.Count > 0 && DistanceExchange.GetOdometerWithRightDistance(refill[0].Odometer) > odometer) {
                odometer = DistanceExchange.GetOdometerWithRightDistance(refill[0].Odometer);
            }

            maintenanceOdometer = DistanceExchange.GetOdometerWithRightDistance(maintenance.Odometer);
            var sub = maintenanceOdometer - odometer;
            if (sub >= 0 && sub < maintenance.WarningDistance)
            {
                this.showReminder(maintenance);
            }
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="maintenance"></param>
        public void Remove(Maintenance maintenance)
        {
            var action = ScheduledActionService.Find(maintenance.Id);
            if (action != null)
            {
                ScheduledActionService.Remove(maintenance.Id);
            }
        }

        /// <summary>
        /// Create reminder
        /// </summary>
        /// <param name="maintenance"></param>
        private void createReminder(Maintenance maintenance)
        {
            var action = ScheduledActionService.Find(maintenance.Id);
            if (action == null) 
            {
                //update begin time
                DateTime beginTime = maintenance.Date.Subtract(TimeSpan.FromDays(maintenance.WarningDays));
                if (beginTime <= DateTime.Now)
                {
                    beginTime = DateTime.Now.AddDays(1);
                }

                //update begin time
                DateTime expirationTime = maintenance.Date;
                if (expirationTime <= DateTime.Now)
                {
                    expirationTime = DateTime.Now.AddDays(1);
                }

                Reminder reminder = new Reminder(maintenance.Id);
                reminder.Title = maintenance.Type.ToString();
                reminder.Content = maintenance.Description;
                reminder.BeginTime = beginTime;
                reminder.ExpirationTime = expirationTime;
                reminder.RecurrenceType = RecurrenceInterval.None;
                reminder.NavigationUri = new Uri("/CoPilot/View/CoPilot.xaml", UriKind.Relative);

                // register the reminder with the system.
                ScheduledActionService.Add(reminder);
            }
        }

        /// <summary>
        /// Show reminder
        /// </summary>
        /// <param name="maintenance"></param>
        private void showReminder(Maintenance maintenance)
        {
            var action = ScheduledActionService.Find(maintenance.Id);
            if (action == null)
            {
                Reminder reminder = new Reminder(maintenance.Id);
                reminder.Title = maintenance.Type.ToString();
                reminder.Content = maintenance.Description;
                reminder.BeginTime = DateTime.Now.Add(TimeSpan.FromSeconds(5));
                reminder.ExpirationTime = DateTime.Now.Add(TimeSpan.FromDays(4));
                reminder.RecurrenceType = RecurrenceInterval.None;
                reminder.NavigationUri = new Uri("/CoPilot/View/CoPilot.xaml", UriKind.Relative);

                // register the reminder with the system.
                ScheduledActionService.Add(reminder);
            }
        }
    }
}
