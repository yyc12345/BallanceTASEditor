using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.ViewModels {

    public partial class NewFileDialog : ObservableObject {
        public NewFileDialog() {
            Count = Shared.Constant.DEFAULT_NEW_COUNT.ToString();
            Fps = Shared.Constant.DEFAULT_FPS.ToString();
        }

        // YYC MARK:
        // 经过无数次的尝试，我发现将int类型绑定到TextBox中所需要涉及的事情太多了，
        // 尤其是这种绑定到处存在于这个程序中，以至于每次都要重新写一遍。
        // 也许后面我会做一个只能接受数字输入的文本框，但现在我累了。
        // 
        // 具体来说，事情是这样的。我一开始就是使用一个int类型的数据，
        // 然后按照CommunityToolkit.Mvvm的标准，将其应用了Required和Range Attribute，然后将其绑定到了TextBox的Text之上。
        // 然而最终的效果很奇怪，当我删除TextBox中的所有字符后，绑定什么也没做（后来我才知道要在Required里改一个选项）。
        // 其次，当我输入不被Range接受的数值时，它仍会将其同步绑定到属性上。比如我输入0，它仍会将其绑定到Fps上，
        // 进而导致Delta Time的转换显示抛出除以零的错误。这些都不是我想要的。
        // 同时，这样做的话，我没有办法检测这些字段到底是不是合规的。
        // 
        // 然后我就将int改写为了int?类型，用null表示当前值不可接受，非null表示绑定的值。
        // 然后定义了一个转换器，负责在int?和string之间进行转换，并同时去除了所有的验证性Attribute
        // 然而遗憾的是，这么做也不符合我的要求。当我尝试输入一个数值的时候，如果我输入了任何无效值，
        // 那么转换器会将其转换为null值，同时将该值赋给Fps，而被赋值的Fps又反向传播，把这个null传递给了转换器，
        // 转换器将其转换成为空白值，并显示在界面上。这么做的视觉效果就是一旦我输入无效值，整个文本框就会被清空，非常的反人类。
        // 而且这一问题是不可调和的，我不能在接收到null时，选择不更新文本框的值，因为有可能这个null是我手动放进去的，而不是从转换器接受的。
        // 
        // 所以最终，我想通了，我决定抛弃将int和TextBox.Text绑定在一起的想法。
        // 就直接把string绑定到TextBox.Text上，然后再辅以我自己定义的一套可复用验证逻辑。

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string count;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string fps;

        //[ObservableProperty]
        ////[CustomValidation(typeof(NewFileDialog), nameof(ValidateCount))]
        //[NotifyCanExecuteChangedFor(nameof(OkCommand))]
        //private string count;

        //[ObservableProperty]
        ////[CustomValidation(typeof(NewFileDialog), nameof(ValidateFps))]
        //[NotifyCanExecuteChangedFor(nameof(OkCommand))]
        //private string fps;

        ////public static ValidationResult ValidateCount(string count, ValidationContext context) {
        ////    return CountValidator.Instance.Validate(count);
        ////}
        ////public static ValidationResult ValidateFps(string fps, ValidationContext context) {
        ////    return FpsValidator.Instance.Validate(fps);
        ////}

        [RelayCommand(CanExecute = nameof(CanOk))]
        private void Ok() {
            OnRequestCloseDialog(true);
        }

        private bool CanOk() {
            // TODO
            return true;
        }

        [RelayCommand]
        private void Cancel() {
            OnRequestCloseDialog(false);
        }


        public event Shared.RequestCloseDialogEventHandler? RequestCloseDialog;

        private void OnRequestCloseDialog(bool result) {
            RequestCloseDialog?.Invoke(new Shared.RequestCloseDialogEventArgs { Result = result});
        }

        //public NewFileDialogResult ToResult() {
        //    return new NewFileDialogResult {
        //        Count = CountValidator.Instance.Fetch(Count),
        //        DeltaTime = FpsConverter.ToDelta(FpsValidator.Instance.Fetch(Fps)),
        //    };
        //}

    }
}
