using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobController : MonoBehaviour {

    public static JobController Instance;

    public Material SpriteMaterial;
    public Sprite JobSprite;

    private List<Job> JobQueue;

    private Dictionary<Job, GameObject> JobGameobjects;

    void Awake() {
        Instance = this;

        JobQueue = new List<Job>();
        JobGameobjects = new Dictionary<Job, GameObject>();
    }

	void Start () {

	}

    public void AddJob(Job job, Tile tile) {
        if(job.GetCompletionTime() <= 0.0f) {
            job.DoJob(0); //Lets autocomplete jobs with 0 or minus completion times. Why should AI waste time if it is gonna complete instantly!
            return; //Job wasn't added to queue
        }

        if(JobQueue.Contains(job)) {
            Debug.Log("JobController::AddJob -> This Job is already in the JobQueue!");
            return;
        }

        if(!tile.SetPendingJob(job))
            return;

        job.RegisterOnCompleteCallback(OnJobEnd);
        job.RegisterOnAbortedCallback(OnJobEnd);

        JobQueue.Add(job);
        JobQueue.Sort(); //Sorts Jobs based on their priority - High to Low

        OnJobCreated(job);
    }

    private void OnJobCreated(Job job) {
        GameObject job_go = new GameObject("Job_" + job.GetTile().GetX() + "_" + job.GetTile().GetY());
        job_go.transform.SetParent(this.transform);
        job_go.transform.position = new Vector3(job.GetTile().GetX(), job.GetTile().GetY(), 0);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
        sr.material = SpriteMaterial;
        sr.sprite = JobSprite;
    }

    private void OnJobEnd(Job job) {
        if(!JobGameobjects.ContainsKey(job)) {
            Debug.Log("SpriteController::OnWorldObjectChanged -> This worldobject doesn't have an associated gameobject!");
            return;
        }

        GameObject job_go = JobGameobjects[job];
        JobGameobjects.Remove(job);

        Destroy(job_go);
    }

}
