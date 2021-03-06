using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobController : MonoBehaviour {

    public static JobController Instance;

    public Material SpriteMaterial;
    public Sprite JobSprite;

	//TODO: Some sort of advanced, intelligent job sorting based on things like priority, creationTime, its requirements, etc.
	private List<QueuedJob> JobQueue; 

    private Dictionary<Job_Old, GameObject> JobGameobjects;

    void Awake() {
        Instance = this;

        JobQueue = new List<QueuedJob>();
        JobGameobjects = new Dictionary<Job_Old, GameObject>();
    }

    public void AddJob(Job_Old job) {
		if(job.GetCompletionTime() <= 0.0f) {
            job.DoJob(0); //Lets autocomplete jobs with 0 or minus completion times. Why should AI waste time if it is gonna complete instantly!
            return;
		}

		QueuedJob queuedJob = new QueuedJob();
		queuedJob.Job = job;
		queuedJob.HasFailed = false;
		queuedJob.JobType = job.GetJobType();

		if(JobQueue.Contains(queuedJob)) {
			Debug.Log("JobController::AddJob -> This Job is already in the JobQueue!");
			return;
		}

		queuedJob.Job.RegisterOnCompleteCallback(OnJobEnd);
		queuedJob.Job.RegisterOnAbortedCallback(OnJobEnd);

		JobQueue.Add(queuedJob);
		JobQueue.Sort(); //Sorts Jobs based on their priority - High to Low

		OnJobCreated(job);
	}

	public void AddFailedJob(Job_Old job) {
		if(job.GetCompletionTime() <= 0.0f) {
			job.DoJob(0); //Lets autocomplete jobs with 0 or minus completion times. Why should AI waste time if it is gonna complete instantly!
			return;
		}

		QueuedJob queuedJob = new QueuedJob();
		queuedJob.Job = job;
		queuedJob.HasFailed = true;
		queuedJob.JobType = job.GetJobType();

		if(JobQueue.Contains(queuedJob)) {
			Debug.Log("JobController::AddJob -> This Job is already in the JobQueue!");
			return;
		}

		queuedJob.Job.RegisterOnCompleteCallback(OnJobEnd);
		queuedJob.Job.RegisterOnAbortedCallback(OnJobEnd);

		JobQueue.Add(queuedJob);
		JobQueue.Sort(); //Sorts Jobs based on their priority - High to Low

		OnJobCreated(job);
	}

	public Job_Old GetJob() {
        if(JobQueue.Count == 0)
            return null;

        QueuedJob queuedJob = JobQueue[0];
        JobQueue.RemoveAt(0);

        return queuedJob.Job;
    }

    public bool HasJob() {
        if(JobQueue.Count == 0)
            return false;

        return true;
    }

    private void OnJobCreated(Job_Old job) {
        if(JobGameobjects.ContainsKey(job))
            return;

        GameObject job_go = new GameObject("Job_" + job.GetTile().GetX() + "_" + job.GetTile().GetY());
        job_go.transform.SetParent(this.transform);
        job_go.transform.position = new Vector3(job.GetTile().GetX(), job.GetTile().GetY(), -0.2f);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
        sr.material = SpriteMaterial;
        sr.sprite = JobSprite;

		JobGameobjects.Add(job, job_go);
    }

    private void OnJobEnd(Job_Old job) {
        if(!JobGameobjects.ContainsKey(job)) {
            Debug.Log("JobController::OnJobEnd -> This Job doesn't have an associated GameObject!");
            return;
        }

        GameObject job_go = JobGameobjects[job];
        JobGameobjects.Remove(job);

        Destroy(job_go);
    }

	public static void CreateBuildJob(string type, Tile tile) {
		ActionMode mode = Defs.GetDef(type).Properties.DefCategory.ToActionMode();
		float jobTime = BuildController.GetJobTime(type, mode);

		Dictionary<string, int> requirements = null;
		if(!string.IsNullOrEmpty(type)) {
			Constructable constructable = (Constructable)Defs.GetDef(type).Properties.Prototype;
			if(constructable != null)
				requirements = constructable.GetConstructionRequirements(type);
		}

		Job_Old job = null;
		if(mode == ActionMode.Tile)
			job = new Job_Old(JobType.Construct, tile, j => tile.ChangeType(type), requirements, jobTime, 0);
		else if(mode == ActionMode.InstalledObject)
			job = new Job_Old(JobType.Construct, tile, j => WorldController.Instance.GetWorld().PlaceInstalledObject(type, tile), requirements, jobTime, 0);

		if(job != null)
			Instance.AddJob(job);
	}

	public static void CreateDismantleJob(Tile tile) {
		string type = tile.GetInstalledObject().GetObjectType();
		float jobTime = BuildController.GetJobTime(type, ActionMode.Dismantle);

		Job_Old job = new Job_Old(JobType.Dismantle, tile, j => WorldController.Instance.GetWorld().DemolishInstalledObject(tile), null, jobTime, 0);
		Instance.AddJob(job);
	}

	public class QueuedJob : IComparable<QueuedJob> {

		public Job_Old Job;
		public bool HasFailed;
		public JobType JobType;

		public int CompareTo(QueuedJob other) {
			return other.Job.CompareTo(this.Job);
		}

	}

}
