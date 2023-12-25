using System;


namespace GameDevProject.Game.Klas_Screens
{

    // Concrete command class
    public class StartButtonCommand : ICommand
    {
        private readonly Action onStartButtonClick;

        public StartButtonCommand(Action onStartButtonClick)
        {
            this.onStartButtonClick = onStartButtonClick;
        }

        public void Execute()
        {
            onStartButtonClick?.Invoke();
        }
    }

}
