using Backend.Core.Database;
using Backend.Core.Factories.CPlayer;
using Backend.Models.FactionModel;
using Backend.Models.InjuryModel;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Backend.Handlers.DeathHandler
{
    public class DeathHandler : Script
    {
        public static List<CPlayer> deadPlayers { get; private set; } = new List<CPlayer>();
        private readonly CDBCLient _database;
        public DeathHandler()
        {
            _database = new CDBCLient();
        }

        [ServerEvent(Event.ResourceStart)]
        public void StartDeathTick()
        {
            Timer timer = new Timer();
            timer.Elapsed += Tick;
            timer.Interval = 60 * 1000;
            timer.Start();

        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(CPlayer player, CPlayer killer, uint reason)
        {
            if (player == null) return;

            deadPlayers.Add(player);
            var injury = new InjuryModel
            {
                TimeLeft = 1,
                Injured = true
            };

            player.DBModel.InjuryModel = injury;

            player.Update().GetAwaiter();

            Task task = Task.Delay(4 * 1000);
            task.Wait();

            PlayDeathAnimation(player);
            
        }

        private void PlayDeathAnimation(CPlayer player)
        {
            if (player != null)
            {
                NAPI.Player.SpawnPlayer(player, player.Position, 0);
                player.SetHealthAC(100);
                Console.WriteLine("ANIM");
                player.PlayAnimation("combat@damage@rb_writhe", "rb_writhe_loop", 8);
            }
        }

        private async void Tick(System.Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("TICKT");

            NAPI.Task.Run(() =>
            {
                deadPlayers.ForEach(p =>
                {
                    if (p == null) return;
                    if (p?.DBModel?.InjuryModel?.TimeLeft - 1 <= 0)
                    {
                        if (p == null) return;

                        p.DBModel.InjuryModel.Injured = false;
                        p.DBModel.InjuryModel.TimeLeft = 0;

                        NAPI.Player.StopPlayerAnimation(p);
                        if (p.DBModel.Faction.name == "Zivilist")
                        {
                            NAPI.Player.SpawnPlayer(p, new Vector3(294.7505798339844, -1449.0618896484375, 29.966590881347656), 0);
                            p.Update().GetAwaiter();
                            if (deadPlayers.Contains(p))
                                deadPlayers.Remove(p);
                            return;
                        }
                        var faction = _database.GetOneFromCollection<FactionModel>("Factions", f => f.name == p.DBModel.Faction.name).Result;
                        if (faction != null)
                        {
                            NAPI.Player.SpawnPlayer(p, faction.SpawnPos, 0);
                            p.SetHealthAC(100);
                        }
                        p.Update().GetAwaiter();
                        if (deadPlayers.Contains(p))
                            deadPlayers.Remove(p);
                    }
                    else
                    {
                        p.DBModel.InjuryModel.TimeLeft -= 1;
                        NAPI.Player.PlayPlayerAnimation(p, 33, "combat@damage@rb_writhe", "rb_writhe_loop", 8f);
                        p.Update().GetAwaiter();
                    }
                });
            });
            
        }
    }
}
