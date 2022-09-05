using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
	public class ViewModel : INotifyPropertyChanged
	{
		private ObservableCollection<CigaretteViewModel> cigarettes;
		public ObservableCollection<CigaretteViewModel> Cigarettes
		{
			get => cigarettes;
			set
			{
				cigarettes = value;
				NotifyPropertyChange("Cigarettes");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChange(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
