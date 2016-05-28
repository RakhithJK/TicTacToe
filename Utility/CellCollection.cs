using System;
using System.Collections;
using System.Collections.Generic;

namespace TicTacToe
{
    public class CellCollection : ViewModelBase, IEnumerable<CellViewModel>
    {
        readonly CellViewModel[,] _cells = new CellViewModel[3,3];
        readonly Dictionary<int, CellViewModel> _cellNumDictionary = new Dictionary<int, CellViewModel>();

        public CellViewModel[] Corners { get; }

        public CellCollection(Action<CellViewModel> OnClick)
        {
            for (var i = 0; i < 3; ++i)
            {
                for (var j = 0; j < 3; ++j)
                {
                    var cell = new CellViewModel((i + 1) * 10 + j + 1);
                    _cells[i,j] = cell;
                    cell.ClickCommand = new DelegateCommand(() => OnClick(cell), () => cell.Occupier == Occupier.None);
                    _cellNumDictionary.Add(cell.CellNum, cell);
                }
            }

            Corners = new[]
            {
                _cells[0,0],
                _cells[0,2],
                _cells[2,0],
                _cells[2,2]
            };
        }

        public CellViewModel this[int i, int j] => _cells[i,j];

        public CellViewModel this[int i] => _cells[i / 3,i % 3];

        public CellViewModel GetByCellNum(int CellNum) => _cellNumDictionary[CellNum];

        public IEnumerator<CellViewModel> GetEnumerator()
        {
            for (var i = 0; i < 3; ++i)
                for (var j = 0; j < 3; ++j)
                    yield return _cells[i,j];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}