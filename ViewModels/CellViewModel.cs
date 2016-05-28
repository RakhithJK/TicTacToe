using System.Windows.Input;

namespace TicTacToe
{
    public class CellViewModel : ViewModelBase
    {
        public CellViewModel(int CellNum)
        {
            this.CellNum = CellNum;
        }

        public ICommand ClickCommand { get; set; }

        public int CellNum { get; }

        Occupier _occupier;

        public Occupier Occupier
        {
            get { return _occupier; }
            set
            {
                if (_occupier == value)
                    return;

                _occupier = value;

                OnPropertyChanged();

                (ClickCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
}