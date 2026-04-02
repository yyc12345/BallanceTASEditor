using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {

    public enum EditorLayoutKind {
        Horizontal, Vertical
    }

    public class AppSettings {



        public EditorLayoutKind EditorLayout { get; set; }

    }
}
