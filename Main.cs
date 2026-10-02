using System;
using Rage;
using LSPD_First_Response.Mod.API;

namespace BluebirdLSPDFR
{
    public class Main : Plugin
    {
        private static bool isTerminalOpen = false;
        private static Rage.Object bluebirdProp = null;

        private static readonly Model propModel = new Model("bluebird");
        private static readonly Model fallbackModel = new Model("prop_npc_phone_02");

        private const string ANIM_DICT = "amb@world_human_stand_mobile@male@text@idle_a";
        private const string ANIM_NAME = "idle_a";

        public override void Initialize()
        {
            Game.LogTrivial("Bluebird Terminal dla LSPDFR zostal zaladowany!");
            GameFiber.StartNew(ProcessLoop);
        }

        public override void Finally()
        {
            DeleteBluebirdProp();
        }

        private static void ProcessLoop()
        {
            while (true)
            {
                GameFiber.Yield();

                if (Game.IsKeyDown(System.Windows.Forms.Keys.F9))
                {
                    ToggleBluebirdTerminal();
                }

                if (Game.IsKeyDown(System.Windows.Forms.Keys.R))
                {
                    LocateLastDispatch();
                }
            }
        }

        private static void ToggleBluebirdTerminal()
        {
            isTerminalOpen = !isTerminalOpen;

            if (isTerminalOpen)
            {
                CreateBluebirdProp();
                Game.DisplayNotification("~b~Bluebird:~w~ Otwarto terminal.");
            }
            else
            {
                DeleteBluebirdProp();
                Game.DisplayNotification("~b~Bluebird:~w~ Zamknieto terminal.");
            }
        }

        private static void CreateBluebirdProp()
        {
            Ped playerPed = Game.LocalPlayer.Character;

            Model modelToUse = propModel.IsValid ? propModel : fallbackModel;
            modelToUse.LoadAndWait();

            if (bluebirdProp != null && bluebirdProp.Exists())
            {
                bluebirdProp.Delete();
            }

            bluebirdProp = new Rage.Object(modelToUse, playerPed.Position);
            bluebirdProp.AttachTo(playerPed, playerPed.GetBoneIndex((PedBoneId)28422), Vector3.Zero, Rotator.Zero);

            playerPed.Tasks.PlayAnimation(ANIM_DICT, ANIM_NAME, 3.0f, AnimationFlags.UpperBodyOnly | AnimationFlags.Loop);
        }

        private static void DeleteBluebirdProp()
        {
            Ped playerPed = Game.LocalPlayer.Character;

            if (playerPed != null && playerPed.Exists())
            {
                playerPed.Tasks.ClearSecondary();
            }

            if (bluebirdProp != null && bluebirdProp.Exists())
            {
                bluebirdProp.Delete();
                bluebirdProp = null;
            }
        }

        private static void LocateLastDispatch()
        {
            Game.DisplayNotification("~g~Bluebird:~w~ Akceptowano zgloszenie (Przycisk R).");
        }
    }
}
