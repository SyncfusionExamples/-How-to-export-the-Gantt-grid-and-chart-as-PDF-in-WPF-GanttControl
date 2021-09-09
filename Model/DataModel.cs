
namespace ExportGanttToPDF
{
    using System;
    using System.Linq;
    using Syncfusion.Windows.Shared;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public class Item : NotificationObject
    {
        #region Fields

        private string _Name;
        private DateTime _StartDate;
        private DateTime _FinishDate;
        private double _progress;
        private ObservableCollection<Item> _subItems;
        private ObservableCollection<Item> _inLineItems;

        #endregion

        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item()
        {
            this._subItems = new ObservableCollection<Item>();
            this._inLineItems = new ObservableCollection<Item>();
            this._subItems.CollectionChanged += this.ItemsCollectionChanged;
            this._inLineItems.CollectionChanged += this.ItemsCollectionChanged;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                this._Name = value;
                this.RaisePropertyChanged("Name");
            }
        }


        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get
            {
                return this._StartDate;
            }

            set
            {
                this._StartDate = value;
                this.RaisePropertyChanged("StartDate");
            }
        }

        /// <summary>
        /// Gets or sets the finish date.
        /// </summary>
        /// <value>The finish date.</value>
        public DateTime FinishDate
        {
            get
            {
                return this._FinishDate;
            }

            set
            {
                this._FinishDate = value;
                this.RaisePropertyChanged("FinishDate");
            }
        }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public double Progress
        {
            get
            {
                return Math.Round(this._progress, 2);
            }

            set
            {
                this._progress = value;
                this.RaisePropertyChanged("Progress");
            }
        }

        /// <summary>
        /// Gets or sets the sub items.
        /// </summary>
        /// <value>The sub items.</value>
        public ObservableCollection<Item> SubItems
        {
            get
            {
                return this._subItems;
            }

            set
            {
                this._subItems = value;
                this._subItems.CollectionChanged += this.ItemsCollectionChanged;
                if (value.Count > 0)
                {
                    this._subItems.ToList().ForEach(n =>
                    {
                        /// To listen the changes occuring in child task.
                        n.PropertyChanged += this.ItemPropertyChanged;
                    });
                    this.UpdateDates();
                }

                this.RaisePropertyChanged("SubItems");
            }
        }

        /// <summary>
        /// Gets or sets the in line items.
        /// </summary>
        /// <value>The in line items.</value>
        public ObservableCollection<Item> InLineItems
        {
            get
            {
                return this._inLineItems;
            }

            set
            {
                this._inLineItems = value;
                this._inLineItems.CollectionChanged += this.ItemsCollectionChanged;
                if (value.Count > 0)
                {
                    this._inLineItems.ToList().ForEach(n =>
                    {
                        /// To listen the changes occuring in child task.
                        n.PropertyChanged += this.ItemPropertyChanged;
                    });
                    this.UpdateDates();
                }

                this.RaisePropertyChanged("InLineItems");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Itemses the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Item item in e.NewItems)
                {
                    item.PropertyChanged += this.ItemPropertyChanged;
                }
            }
            else
            {
                foreach (Item item in e.OldItems)
                {
                    item.PropertyChanged -= this.ItemPropertyChanged;
                }
            }

            this.UpdateDates();
        }

        /// <summary>
        /// Items the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null)
            {
                if (e.PropertyName == "StartDate" || e.PropertyName == "FinishDate" || e.PropertyName == "Progress")
                {
                    this.UpdateDates();
                }
            }
        }

        /// <summary>
        /// Updates the dates.
        /// </summary>
        private void UpdateDates()
        {
            var tempCal = 0d;
            if (this._subItems.Count > 0)
            {
                /// Updating the start and end date based on the chagne occur in the date of child task
                this.StartDate = this._subItems.Select(c => c.StartDate).Min();
                this.FinishDate = this._subItems.Select(c => c.FinishDate).Max();
                this.Progress = this._subItems.Aggregate(tempCal, (cur, task) => cur + task.Progress) / this._subItems.Count;
            }

            if (this._inLineItems.Count > 0)
            {
                /// Updating the start and end date based on the chagne occur in the date of child task
                this.StartDate = this._inLineItems.Select(c => c.StartDate).Min();
                this.FinishDate = this._inLineItems.Select(c => c.FinishDate).Max();
                this.Progress = this._inLineItems.Aggregate(tempCal, (cur, task) => cur + task.Progress) / this._inLineItems.Count;
            }
        }

        #endregion
    }
}