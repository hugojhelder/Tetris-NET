using System;
using System.Windows.Input;
using System.Windows.Threading;
using TetrisClient.Logic;
using TetrisClient.Ui.Controls;

namespace TetrisClient.Controller
{
    public interface IGameController
    {
        /// <summary>
        /// Property that gets the timer.
        /// </summary>
        public TetrisControl Control { get; }
        
        /// <summary>
        /// Property that gives the instance of the game
        /// </summary>
        public Game Game { get; }

        /// <summary>
        /// Property that says if the shift key is down.
        /// </summary>
        public bool ShiftDown { get; set; }
        
        /// <summary>
        /// Property that gets the timer.
        /// </summary>
        public DispatcherTimer Timer { get; }
        
        /// <summary>
        /// Cleans everything up after a game has been played.
        /// </summary>
        public void CleanUp();
        
        /// <summary>
        /// Clears all fields
        /// </summary>
        public void ClearFields();

        /// <summary>
        /// Sets all the fields
        /// </summary>
        public void SetFields();
        
        /// <summary>
        /// Checks if the tick speed should be incremented.
        /// </summary>
        /// <returns></returns>
        public bool ShouldIncrementTickSpeed();

        /// <summary>
        /// Increments the tick speed
        /// </summary>
        public void IncrementTickSpeed();
        
        #region Events

        /// <summary>
        /// Handles key down events. 
        /// </summary>
        /// <param name="e"></param>
        public void On_KeyDown(KeyEventArgs e);
        
        /// <summary>
        /// Handles key up events.
        /// </summary>
        /// <param name="e"></param>
        public void On_KeyUp(KeyEventArgs e);
        
        /// <summary>
        /// Handles starting the game.
        /// </summary>
        public void On_StartBtnClick();
        

        /// <summary>
        /// Handles stopping the game
        /// </summary>
        public void On_StopBtnClick();

        /// <summary>
        /// Handles the ticking of the timer
        /// </summary>
        public void On_TimerTick(object sender, EventArgs e);

        #endregion
    }
}
