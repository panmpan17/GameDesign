using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using MPack;

public class SubtitleClip : PlayableAsset
{
    [LauguageID]
    public int LanguageID;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playerble = ScriptPlayable<SubtitleBehaviour>.Create(graph);

        var behaviour = playerble.GetBehaviour();
        behaviour.LanguageID = LanguageID;

        return playerble;
    }
}
