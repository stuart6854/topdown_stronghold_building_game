using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobController : MonoBehaviour {

    public static JobController Instance;

    private List<Job> JobQueue;

    void Awake() {
        Instance = this;

        JobQueue = new List<Job>();
    }

	void Start () {

	}

    public bool AddJob(Job job) {
        if(job.GetCompletionTime() <= 0.0f) {
            job.DoJob(0); //Lets autocomplete jobs with 0 or minus completion times. Why should AI waste time if it is gonna complete instantly!
            return false; //Job wasn't added to queue
        }

        if(JobQueue.Contains(job)) {
            Debug.Log("JobController::AddJob -> This Job is already in the JobQueue!");
            return false;
        }

        JobQueue.Add(job);

        JobQueue.Sort(); //Sorts Jobs based on their priority - High to Low
        return true;
    }

}
