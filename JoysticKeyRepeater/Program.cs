using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using System.Threading;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace JoysticKeyRepeater
{
    class Program
    {

        static void Main()
        {
            ViGEmClient client = new ViGEmClient();
            IDualShock4Controller virtualController = client.CreateDualShock4Controller();
            bool active = false;
            var pressSleep = 200;

            var clickerTask =  Task.Run(() =>
            {
                while (true)
                {
                    if (active) { 
                        //true or false
                        virtualController.SetButtonState(DualShock4Button.Circle, true);
                        Thread.Sleep(pressSleep);
                        virtualController.SetButtonState(DualShock4Button.Circle, false);
                        Thread.Sleep(pressSleep);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                    
                }
            });


        // Initialize DirectInput
        var directInput = new DirectInput();

        // Find a Joystick Guid


        scanDev:

            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
            {
                joystickGuid = deviceInstance.InstanceGuid;
                Console.WriteLine("Found Guid:" + joystickGuid);
            }

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;
            //
            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                //Console.WriteLine("No joystick/Gamepad found.");
                Thread.Sleep(1000);
                //Console.ReadKey();
                //Environment.Exit(1);
                goto scanDev;
            }

            // Instantiate the joystick
            var joystick = new Joystick(directInput, joystickGuid);
            //joystick.SetCooperativeLevel(0, CooperativeLevel.NonExclusive | CooperativeLevel.Background);
            Console.WriteLine("Found Joystick/Gamepad with GUID: {0}", joystickGuid);

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                 Console.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Unacquire();
            joystick.Acquire();
            //var s = joystick.get();
            // Poll events from joystick
            JoystickState lastState = new JoystickState();
            while (true)
            {

                //state.
                //joystick.Poll();
                try
                {
                    joystick.Poll();
                    JoystickState state = joystick.GetCurrentState();
                    /*JoystickState state = joystick.GetCurrentState();
                    if (lastState.Equals(state)) {
                        goto skip;
                    }
                    Console.WriteLine(state);
                    lastState = state;

                     */

                    JoystickUpdate[] updates = joystick.GetBufferedData();

                    if (updates.Length > 0)
                    {
                        //if has an update related to button4(DS4-L1) and keyup event
                        var uB4 = updates.ToList().Exists(u => u.Offset == JoystickOffset.Buttons4 && u.Value == 0);

                        //if has an update related to button4(DS4-R1) and keyup event
                        var uB5 = updates.ToList().Exists(u => u.Offset == JoystickOffset.Buttons5 && u.Value == 0);

                        if (uB4 && uB5) {
                            if (!active)
                            {
                                Console.Beep(3000, 300);
                                Console.WriteLine("Activate Cliker!");
                                virtualController.Connect();

                                active = true;
                            }
                            else {
                                Console.Beep(2000, 500);
                                Console.WriteLine("Deactivate Cliker!");

                                active = false;
                                virtualController.Disconnect();

                            }
                        }

                       /*foreach (var u in updates)
                        {
                            Console.WriteLine(u);

                        }*/
                    }



                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    joystick.Unacquire();
                    joystick.Dispose();
                    goto scanDev;
                }

                Thread.Sleep(100);
                /*foreach (var state in datas)
                    Console.WriteLine(state[0]);*/
            }
        }
    }
}
