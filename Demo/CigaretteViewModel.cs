using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
	public class CigaretteViewModel : INotifyPropertyChanged
	{
		string id;

		public string Id
		{
			get => id;
			set
			{
				id = value;
				NotifyPropertyChange("Id");
			}
		}

		string name;

		public string Name
		{
			get => name;
			set
			{
				name = value;
				NotifyPropertyChange("Name");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChange(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
