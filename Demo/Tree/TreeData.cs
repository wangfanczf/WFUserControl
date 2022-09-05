using System.Collections.ObjectModel;
using System.ComponentModel;
using WFUserControl;

namespace Demo
{
    public class TreeData : INotifyPropertyChanged
    {
        private ObservableCollection<TreeViewItemViewModelBase> nodes;
        public ObservableCollection<TreeViewItemViewModelBase> Nodes 
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new ObservableCollection<TreeViewItemViewModelBase>();
                }
                return nodes;
            }

            set
            {
                nodes = value;
                NotifyPropertyChange(nameof(Nodes));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
