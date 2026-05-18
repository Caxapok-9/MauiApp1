using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using SQLiteNetExtensions.Attributes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class Player : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName => $"{Number} - {Name}";

        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; } = null;

        public string Name { get; set; }

        public string Number { get; set; }

        [ForeignKey(typeof(Team))]
        public int TeamID { get; set; }

        [ForeignKey(typeof(Player))]
        public int ReplaceID { get; set; } = 0;

        public bool IsDisqual {  get; set; }

        public bool IsRemove { get; set; }

        public bool IsCaptain
        {
            get => _IsCaptain;
            set
            {
                if (_IsCaptain != value) // Меняем только если значение другое
                {
                    _IsCaptain = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        public bool IsLibero
        {
            get => _IsLibero;
            set
            {
                if (_IsLibero != value) // Меняем только если значение другое
                {
                    _IsLibero = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private bool _IsCaptain { get; set; }

        private bool _IsLibero { get; set; }

        public Player()
        {
            Number = "";
            Name = "";
            _IsCaptain = false;
            _IsLibero = false;
        }

    }
}
