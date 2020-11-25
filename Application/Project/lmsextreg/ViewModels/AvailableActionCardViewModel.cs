namespace lmsextreg.ViewModels
{
    public class AvailableActionCardViewModel
    {
        public AvailableActionCardViewModel(string title, string text, string buttonLabel, string onSubmit, string cardId, string buttonId)
        {
            this.Title          = title;
            this.Text           = text;
            this.ButtonLabel    = buttonLabel;
            this.OnSubmit       = onSubmit;
            this.CardId         = cardId;
            this.ButtonId       = buttonId;
        }

        public string Title         { get; }
        public string Text          { get; }
        public string ButtonLabel   { get; }
        public string OnSubmit      { get; }
        public string CardId        { get; }
        public string ButtonId      { get; }
    }
}