using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Shared {

    /// <summary>
    /// The delegate used for view model requesting closing view window (non-modal window).
    /// </summary>
    public delegate void RequestCloseWindowEventHandler();

    /// <summary>
    /// The delegate used for view model requesting closing view dialog (modal window).
    /// </summary>
    public delegate void RequestCloseDialogEventHandler(RequestCloseDialogEventArgs e);

    /// <summary>
    /// The payload passed when requesting closing view dialog (modal window).
    /// </summary>
    public record RequestCloseDialogEventArgs {
        /// <summary>
        /// True if we want to close windows by clicking Ok button, otherwise false.
        /// </summary>
        public required bool Result { get; init; }
    }

}
