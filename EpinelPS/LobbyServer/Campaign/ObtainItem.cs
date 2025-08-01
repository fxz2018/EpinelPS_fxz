﻿using EpinelPS.Database;
using EpinelPS.LobbyServer.Stage;
using EpinelPS.Data;
using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Campaign
{
    [PacketPath("/campaign/obtain/item")]
    public class ObtainItem : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            ReqObtainCampaignItem req = await ReadData<ReqObtainCampaignItem>();
            User user = GetUser();

            ResObtainCampaignItem response = new();

            if (!user.FieldInfoNew.TryGetValue(req.MapId, out FieldInfoNew? field))
            {
                field = new FieldInfoNew();
                user.FieldInfoNew.Add(req.MapId, field);
            }


            foreach (NetFieldObject item in field.CompletedObjects)
            {
                if (item.PositionId == req.FieldObject.PositionId)
                {
                    Logging.WriteLine("attempted to collect campaign field object twice!", LogType.WarningAntiCheat);
                    return;
                }
            }

            // Register and return reward

            MapInfo map = GameData.Instance.MapData[req.MapId];

            ItemSpawner position = map.ItemSpawner.Where(x => x.positionId == req.FieldObject.PositionId).FirstOrDefault() ?? throw new Exception("bad position id");

            FieldItemRecord positionReward = GameData.Instance.FieldItems[position.itemId];
            RewardTableRecord reward = GameData.Instance.GetRewardTableEntry(positionReward.type_value) ?? throw new Exception("failed to get reward");
            response.Reward = RewardUtils.RegisterRewardsForUser(user, reward);

            // Hide it from the field
            field.CompletedObjects.Add(new NetFieldObject() { PositionId = req.FieldObject.PositionId, Type = req.FieldObject.Type});

            JsonDb.Save();

            await WriteDataAsync(response);
        }
    }
}
