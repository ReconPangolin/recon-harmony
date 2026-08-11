using Content.Shared._Funkystation.Botany.PlantAnalyzer;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Botany.UI
{
    [UsedImplicitly]
    public sealed class PlantAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
    {
        [ViewVariables]
        private PlantAnalyzerWindow? _window;

        protected override void Open()
        {
            base.Open();

            _window = this.CreateWindow<PlantAnalyzerWindow>();
            _window.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;
        }


        protected override void ReceiveMessage(BoundUserInterfaceMessage message)
        {
            if (_window == null)
                return;

            if (message is not PlantAnalyzerUserMessage cast)
                return;

            _window.Populate(cast);
        }

    }
}
