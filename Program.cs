using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Management;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Diagnostics;

namespace trolling_freshman
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uparam, string vParam, UInt32 winIni); // DLL and function needed for wallpaper change
        static void Main(string[] args)
        {
            var vm_status = vmDetect();
            kill_wallpaperengine();
            SetWP(Environment.CurrentDirectory + "\\background.png");
            MaxVol();
            // Sleep to hide itself from the user...
            //Check to see if system version is Windows         
            if (Environment.OSVersion.ToString().Contains("Microsoft Windows") && !vm_status)
            { 
                //Play Sound
                var string_path = Environment.CurrentDirectory + "\\audio.wav";
                var audio_file = File.ReadAllBytes(string_path);
                string b64encoded_audio = Convert.ToBase64String(audio_file);
                    
                //Show Message box
                string message_to_show = "Don't Plug in Unknown Usbs You never know What they might do....\nHappy Cyber Security Awareness Month :)";
                
                PlaySound(b64encoded_audio);
                MessageBox.Show(message_to_show);
                

            }
        }

        static void MaxVol()
            {
                CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
                if (defaultPlaybackDevice.IsMuted)
                {
                    defaultPlaybackDevice.ToggleMute();
                }
                defaultPlaybackDevice.Volume = 100;
            }


            static void PlaySound(string base64String)
            {
                var audioBuffer = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(audioBuffer))
                {
                    var player = new System.Media.SoundPlayer(ms);
                    player.Play();
                }
            }

            static void kill_wallpaperengine()
        {
            foreach(var process in Process.GetProcessesByName("wallpaper32"))
            {
                process.Kill();
            }
        }


            static void SetWP(string path)
            {

                SystemParametersInfo(0x14, 0, path, 0x01 | 0x02);
            }

            static bool vmDetect()
            {
                // create management class object
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                //collection to store all management objects
                ManagementObjectCollection moc = mc.GetInstances();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        // Super simple check to see if Manufacturer name contains VM
                        string manufacturer = mo["Manufacturer"].ToString();
                        if (manufacturer.Contains("VM"))
                        {
                            return true;
                        }
                    }


                }
                return false;
            }
        }
    }