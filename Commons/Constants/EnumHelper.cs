﻿namespace Golbaus_BE.Commons.Constants
{
	public enum Gender
	{
		Unknown = 0,
		Male = 1,
		Female = 2
	}

	public enum Role
	{
		SuperAdmin = 0,
		Admin = 1,
		User = 2
	}

	public enum PublishType
	{
		Public = 0,
		Schedule = 1,
		Private = 2
	}

	public enum VoteType
	{
		UpVote = 0,
		DownVote = 1,
		Unvote = 2,
	}

	public enum OrderBy
	{
		PublishDate = 0,
		Vote = 1,
		View = 2,
	}

	public enum OrderType
	{
		Ascending = 0,
		Descending = 1,
	}

	public enum NotificationType
	{
		NewPost = 0,
		NewQuestion = 1,
		Follow = 2,
		CommentPost = 3,
		AnswerQuestion = 4,
		Reply = 5,
	}
}
