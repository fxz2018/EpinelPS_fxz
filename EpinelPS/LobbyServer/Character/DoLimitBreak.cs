﻿using EpinelPS.Database;
using EpinelPS.Data;
using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Character
{
    [PacketPath("/character/upgrade")]
    public class DoLimitBreak : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            // Read the incoming request that contains the current CSN and ISN
            ReqCharacterUpgrade req = await ReadData<ReqCharacterUpgrade>(); // Contains csn and isn (read-only)
            ResCharacterUpgrade response = new();
            User user = GetUser();

            // Get all character data from the game's character table
            List<CharacterRecord> fullchardata = [.. GameData.Instance.CharacterTable.Values];

            CharacterModel targetCharacter = user.GetCharacterBySerialNumber(req.Csn) ?? throw new NullReferenceException();

            // Find the element with the current csn from the request
            CharacterRecord currentCharacter = fullchardata.FirstOrDefault(c => c.id == targetCharacter.Tid) ?? throw new NullReferenceException();

            if (currentCharacter != null && targetCharacter != null)
            {
                if (currentCharacter.grade_core_id == 103 || currentCharacter.grade_core_id == 11 || currentCharacter.grade_core_id == 201)
                {
                    Console.WriteLine("cannot limit break any further!");
                    await WriteDataAsync(response);
                    return;
                }

                // Find a new CSN based on the `name_code` of the current character and `grade_core_id + 1`
                // For some reason, there is a seperate character for each limit/core break value.
                CharacterRecord? newCharacter = fullchardata.FirstOrDefault(c => c.name_code == currentCharacter.name_code && c.grade_core_id == currentCharacter.grade_core_id + 1);


                if (newCharacter != null)
                {
                    // replace character in DB with new character
                    targetCharacter.Grade++;
                    targetCharacter.Tid = newCharacter.id;

                    response.Character = new NetUserCharacterDefaultData()
                    {
                        Csn = req.Csn,
                        CostumeId = targetCharacter.CostumeId,
                        Grade = targetCharacter.Grade,
                        Lv = user.GetSynchroLevel(),
                        Skill1Lv = targetCharacter.Skill1Lvl,
                        Skill2Lv = targetCharacter.Skill2Lvl,
                        Tid = targetCharacter.Tid,
                        UltiSkillLv = targetCharacter.UltimateLevel
                    };

                    // remove spare body item
                    ItemData bodyItem = user.Items.FirstOrDefault(i => i.Isn == req.Isn) ?? throw new NullReferenceException();
                    user.RemoveItemBySerialNumber(req.Isn, 1);
                    response.Items.Add(NetUtils.ToNet(bodyItem));

                    JsonDb.Save();
                }
            }

            // Send the response back to the client
            await WriteDataAsync(response);
        }
    }
}
