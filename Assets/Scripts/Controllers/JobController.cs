using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobController : MonoBehaviour {

    public static JobController Instance;

    public Material SpriteMaterial;
    public Sprite JobSprite;

    private List<Job> JobQueue; //TODO: Some sort of advanced, intelligent job sorting based on things like priority, creationTime, its requirements, etc.

    private Dictionary<Job, GameObject> JobGameobjects;

    void Awake() {
        Instance = this;

        JobQueue = new List<Job>();
        JobGameobjects = new Dictionary<Job, GameObject>();
    }

    public void AddJob(Job job) {
		if(job.GetCompletionTime() <= 0.0f) {
            job.DoJob(0); //Lets autocomplete jobs with 0 or minus completion times. Why should AI waste time if it is gonna complete instantly!
            return;
		}

		if(JobQueue.Contains(job)) {
            Debug.Log("JobController::AddJob -> This Job is already in the JobQueue!");
            return;
		}

		job.ResetTimeCreated();

        job.RegisterOnCompleteCallback(OnJobEnd);
        job.RegisterOnAbortedCallback(OnJobEnd);

		JobQueue.Add(job);
        JobQueue.Sort(); //Sorts Jobs based on their priority - High to Low

		OnJobCreated(job);
    }

	public void AddFailedJob(Job job) {
		if(job.GetCompletionTime() <= 0.0f) {
			job.DoJob(0); //Lets autocomplete jobs with 0 or minus completion times. Why should AI waste time if it is gonna complete instantly!
			return;
		}

		if(JobQueue.Contains(job)) {
			Debug.Log("JobController::AddJob -> This Job is already in the JobQueue!");
			return;
		}

		job.RegisterOnCompleteCallback(OnJobEnd);
		job.RegisterOnAbortedCallback(OnJobEnd);

		QueuedJob queuedJob = new QueuedJob();
		queuedJob.Job = job;
		queuedJob.HasFailed = true;
		queuedJob.JobType = job.GetJobType();

//		JobQueue.Add(queuedJob);
//		JobQueue.Sort(); //Sorts Jobs based on their priority - High to Low

		OnJobCreated(job);
	}

	public Job GetJob() {
        if(JobQueue.Count == 0)
            return null;

        Job job = JobQueue[0];
        JobQueue.RemoveAt(0);

        return job;
    }

    public bool HasJob() {
        if(JobQueue.Count == 0)
            return false;

        return true;
    }

    private void OnJobCreated(Job job) {
        if(JobGameobjects.ContainsKey(job))
            return;

        GameObject job_go = new GameObject("Job_" + job.GetTile().GetX() + "_" + job.GetTile().GetY());
        job_go.transform.SetParent(this.transform);
        job_go.transform.position = new Vector3(job.GetTile().GetX(), job.GetTile().GetY(), 0);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
        sr.material = SpriteMaterial;
        sr.sprite = JobSprite;

		JobGameobjects.Add(job, job_go);
    }

    private void OnJobEnd(Job job) {
        if(!JobGameobjects.ContainsKey(job)) {
            Debug.Log("JobController::OnJobEnd -> This Job doesn't have an associated GameObject!");
            return;
        }

        GameObject job_go = JobGameobjects[job];
        JobGameobjects.Remove(job);

        Destroy(job_go);
    }

	public class QueuedJob {

		public Job Job;
		public bool HasFailed;
		public JobType JobType;

	}

}
