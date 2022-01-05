using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] m_MusicTracks;
    public int m_PreviousTrackNumber = 1000;

    private void Awake()
    {
        GameObject[] l_MusicManagers = GameObject.FindGameObjectsWithTag("Manager.Music");
        if (l_MusicManagers.Length > 1)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        GetRandom();
    }

    void GetRandom()
    {
        TryRandom((int)Random.Range(0, m_MusicTracks.Length - 1));
    }

    void TryRandom(int a_Random)
    {
       

        if (a_Random < m_PreviousTrackNumber)
            PlayTrack(a_Random);
        else if (a_Random >= m_PreviousTrackNumber)
            PlayTrack(a_Random + 1);
    }

    void PlayTrack(int a_TrackNumber)
    {
        Debug.Log("Playing: " + a_TrackNumber);

        m_PreviousTrackNumber = a_TrackNumber;
        GetComponent<AudioSource>().clip = m_MusicTracks[a_TrackNumber];
        GetComponent<AudioSource>().Play();
        StartCoroutine(WaitForEnd(GetComponent<AudioSource>().clip.length));
    }


    IEnumerator WaitForEnd(float a_TrackLength)
    {
        Debug.Log("TrackWait");
        yield return new WaitForSeconds(a_TrackLength);
        Debug.Log("TrackEnd");
        GetRandom();
    }
}
