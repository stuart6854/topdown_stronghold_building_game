using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobHandler {

	private static List<Job> jobs = new List<Job>();

	/// <summary>
	/// Finds and returns the job which is best suited for the requesting character.
	///  </summary>
	/// <param name="_character"></param>
	/// <returns></returns>
	public static Job GetBestJob(Character _character) {
		if(jobs.Count == 0)
			return null; //Currently no jobs

		Job bestJob = null;

		List<SortedJob> sortedJobs = CreateSortedList(_character);
		bestJob = sortedJobs[0].Job;

		jobs.Remove(bestJob);

		return bestJob;
	}

	private static List<SortedJob> CreateSortedList(Character _character) {
		List<SortedJob> sortedJobs = new List<SortedJob>();

		//TODO: Filter out jobs that the character CANT do

		foreach(Job job in jobs) {
			SortedJob sortedJob = new SortedJob();
			sortedJob.Job = job;
			sortedJob.Priority = job.GetPriority();
			sortedJob.HasFailed = job.GetHasFailedBefore();
			sortedJob.JobType = job.GetJobType();
			sortedJob.EuclideanDistance = sortedJob.GetEuclideanDistanceTo(_character.GetCurrentTile());

			sortedJobs.Add(sortedJob);
		}

		sortedJobs.Sort();
		return sortedJobs;
	}

	public static void AddJob(Job _job) {
		if(jobs.Contains(_job))
			return;

		if(!_job.GetTile().AssignPendingJob(_job))
			return;

		jobs.Add(_job);
		SpriteController.Instance.OnJobCreated(_job);
	}

	private class SortedJob : IComparable {

		//NOTE TO SELF: Keep factors in order of importance
		public bool HasFailed; // If a character fails job this will be true
		public short Priority;
		public JobTypes JobType;
		public float EuclideanDistance; //The straight distance between the job and the character

		public Job Job;

		public float GetEuclideanDistanceTo(Tile _from) {
			Vector2 fromVec = new Vector2(_from.GetX(), _from .GetY());
			Vector2 toVec = new Vector2(Job.GetTile().GetX(), Job.GetTile().GetY());

			return Vector2.Distance(fromVec, toVec);
		}

		public int CompareTo(object obj) {
			// -1 = this is smaller
			// 0 = this is equal
			// 1 = this is larger

			SortedJob other = obj as SortedJob;
			if(other == null)
				return 0;

			// This could be wrong??
			int res = this.HasFailed.CompareTo(other.HasFailed);
			if(res == 0)
				res = -this.Priority.CompareTo(other.Priority); // We want Higher Numbers to be first
			if(res == 0)
				res = - this.JobType.CompareTo(other.JobType); // We want Higher Numbers to be first
			if(res == 0)
				res = this.EuclideanDistance.CompareTo(other.EuclideanDistance);
			return res;
		}

	}

}
