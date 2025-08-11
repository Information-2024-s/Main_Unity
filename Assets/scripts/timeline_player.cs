using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class cartain_player : MonoBehaviour
{
    private AlembicStreamPlayer player;
    public play_type_enum play_type;

    public enum play_type_enum
    {
        normal_loop, bounce
    }
    public float start;
    public float end;
    private bool forward = true;
    void Start()
    {
        player = GetComponent<AlembicStreamPlayer>();
        player.CurrentTime = start;
    }

    void Update()
    {
        if (player != null)
        {
            switch (play_type)
            {
                case play_type_enum.normal_loop:
                    if (player.CurrentTime < end)
                        player.CurrentTime += Time.deltaTime;
                    else
                        player.CurrentTime = start;                             
                    break;
                case play_type_enum.bounce:
                    if (player.CurrentTime >= end)
                        forward = false;
                    else if (player.CurrentTime <= start)
                        forward = true;
                    if (forward)
                            player.CurrentTime += Time.deltaTime;
                        else
                            player.CurrentTime -= Time.deltaTime;    
                    break;
            }
        }
    }
}