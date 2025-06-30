using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.Formats.Alembic.Importer;
#endif

public class AlembicRuntimePlayer : MonoBehaviour
{
#if UNITY_EDITOR
    public AlembicStreamPlayer alembicPlayer;
#endif

    public bool autoPlay = true;
    public float playbackSpeed = 1.0f;
    public bool loop = true;

#if UNITY_EDITOR
    private float currentTime = 0f;
#endif

    void Start()
    {
#if UNITY_EDITOR
        if (alembicPlayer == null)
        {
            alembicPlayer = GetComponent<AlembicStreamPlayer>();
        }

        currentTime = 0f;
        alembicPlayer.UpdateImmediately(currentTime);
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!autoPlay || alembicPlayer == null)
            return;

        currentTime += Time.deltaTime * playbackSpeed;

        if (loop && currentTime > alembicPlayer.Duration)
        {
            currentTime = 0f;
        }

        alembicPlayer.UpdateImmediately(currentTime);
#endif
    }
}
