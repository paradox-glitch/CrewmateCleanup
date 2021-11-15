using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameObject
        m_DropDecalPool,
        m_SplatDecalPool;
    [SerializeField]
    private int
        m_StartDecals,
        m_StartSmallJunk,
        m_StartLargeJunk,
        m_StartChildren,
        m_StartRats,
        m_StartTotalPoints;

    [SerializeField]
    private int
        m_CurrentDecals,
        m_CurrentSmallJunk,
        m_CurrentLargeJunk,
        m_CurrentChildren,
        m_CurrentRats;

    [SerializeField]
    private int
        m_PointsDecals = 1,
        m_PointsSmallJunk = 1,
        m_PointsLargeJunk = 10,
        m_PointsChildren = 5,
        m_PointsRats = 12;

    public float persentofdirt = 100f;

    [SerializeField]
    private LayerMask DisposalAreaLayer;

    // Start is called before the first frame update
    void Start()
    {
        m_DropDecalPool = GameObject.FindGameObjectWithTag("DropDecalPool");
        m_SplatDecalPool = GameObject.FindGameObjectWithTag("SpaltDecalPool");

        StartCoroutine(DelayedStart());

    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        m_CurrentDecals = m_StartDecals = DecalCheck();
        m_CurrentSmallJunk = m_StartSmallJunk = GameObject.FindGameObjectsWithTag("Dirt.SmallJunk").Length;
        m_CurrentLargeJunk = m_StartLargeJunk = LargeJunkCheck();
        m_CurrentChildren = m_StartChildren = GameObject.FindGameObjectsWithTag("Dirt.Kid").Length;
        m_CurrentRats = m_StartRats = GameObject.FindGameObjectsWithTag("Dirt.Rat").Length;

        m_StartTotalPoints = DoMath();
    }

    // Update is called once per frame
    void Update()
    {

    }

    int DoMath()
    {
        int TotalPoints = 0;
        TotalPoints += (m_CurrentDecals * m_PointsDecals);
        TotalPoints += (m_CurrentSmallJunk * m_PointsSmallJunk);
        TotalPoints += (m_CurrentLargeJunk * m_PointsLargeJunk);
        TotalPoints += (m_CurrentChildren * m_PointsChildren);
        TotalPoints += (m_CurrentRats * m_PointsRats);
        return TotalPoints;
    }

    float DoOtherMaths()
    {
        float per = 100f;

        float worth = 100f / m_StartTotalPoints;

        Debug.Log(worth);

        per = worth * DoMath();

        return per;
    }

    void DoCheck()
    {
        m_CurrentDecals = DecalCheck();
        m_CurrentSmallJunk = GameObject.FindGameObjectsWithTag("Dirt.SmallJunk").Length;
        m_CurrentLargeJunk = LargeJunkCheck();
        m_CurrentChildren = GameObject.FindGameObjectsWithTag("Dirt.Kid").Length;
        m_CurrentRats = GameObject.FindGameObjectsWithTag("Dirt.Rat").Length;

        persentofdirt = DoOtherMaths();
    }

    int LargeJunkCheck()
    {
        int l_LargeJunkLeft = 0;

        GameObject[] l_LargeJunk = GameObject.FindGameObjectsWithTag("Dirt.DeadBody");
        foreach(GameObject a_Junk in l_LargeJunk)
        {
            if (Physics.OverlapSphere(a_Junk.transform.position, 2, DisposalAreaLayer).Length == 0)
                l_LargeJunkLeft++;
        }

        return l_LargeJunkLeft;
    }

    int DecalCheck()
    {
        int l_DecalsLeft = 0;

        l_DecalsLeft += m_DropDecalPool.GetComponent<ParticleDecalPool>().DecalsLeft();

        l_DecalsLeft += m_SplatDecalPool.GetComponent<ParticleDecalPool>().DecalsLeft();

        return l_DecalsLeft;
    }

    private void OnDrawGizmos()
    {
        GameObject[] l_LargeJunk = GameObject.FindGameObjectsWithTag("Dirt.DeadBody");
        foreach (GameObject a_Junk in l_LargeJunk)
        {
            Gizmos.DrawWireSphere(a_Junk.transform.position, 2);
        }
    }
}
