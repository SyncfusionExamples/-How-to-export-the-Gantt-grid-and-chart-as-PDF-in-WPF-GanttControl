
namespace ExportGanttToPDF
{
    using System.Collections.ObjectModel;
    using Syncfusion.Windows.Controls.Gantt;

    public class ViewModel : ProjectGanttTaskRepository
    {
        #region Fields

        private ObservableCollection<TaskDetails> _taskCollection;
        private ObservableCollection<Item> _teamDetails;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this._taskCollection = this.GetData();
            this._teamDetails = this.GetTeamInfo();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the appointment item source.
        /// </summary>
        /// <value>The appointment item source.</value>
        public ObservableCollection<TaskDetails> TaskCollection
        {
            get
            {
                return this._taskCollection;
            }

            set
            {
                this._taskCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the appointment item source.
        /// </summary>
        /// <value>The appointment item source.</value>
        public ObservableCollection<Item> TeamDetails
        {
            get
            {
                return this._teamDetails;
            }

            set
            {
                this._teamDetails = value;
            }
        }

        #endregion
    }
}