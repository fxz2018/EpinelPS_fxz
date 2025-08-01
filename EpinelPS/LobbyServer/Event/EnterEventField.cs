﻿using EpinelPS.Database;
using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Event
{
    [PacketPath("/eventfield/enter")]
    public class EnterEventField : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            ReqEnterEventField req = await ReadData<ReqEnterEventField>();
            User user = GetUser();

            ResEnterEventField response = new()
            {
                Field = new(),
                Json = "{}"
            };

            

            // Retrieve collected objects

            if (!user.FieldInfoNew.TryGetValue(req.MapId, out FieldInfoNew? field))
            {
                field = new FieldInfoNew();
                user.FieldInfoNew.Add(req.MapId, field);
            }

            foreach (int stage in field.CompletedStages)
            {
                response.Field.Stages.Add(new NetFieldStageData() { StageId = stage });
            }
            foreach (NetFieldObject obj in field.CompletedObjects)
            {
                response.Field.Objects.Add(obj);
            }


            // Retrieve camera data
            if (user.MapJson.TryGetValue(req.MapId, out string? mapJson))
            {
                response.Json = mapJson;
            }

            await WriteDataAsync(response);
        }
    }
}
