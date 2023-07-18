using System.Collections.Concurrent;


namespace RcpEstimator
{



    public class ScheduleEstimator
    {

        const string AlgorithmnException = "Algorithmn Estimate Error!!!";


        public int NumOfTasks = 0;

        public int[] TaskDuration;
        public int[][] TaskAdjacency;
        public int[][] TaskExper;

        public int[][] TaskOfStartFinishTime;
        public int[][] TaskSortedUnitTime;
        public List<int> StortedUnitTimeList;

        public List<int> StartTasks = new List<int>();
        int[] TaskMilestone;
        // group id tasks's by milestoneid
        Dictionary<int, List<int>> GroupedTaskMilestone;


        public ScheduleEstimator(int[] TaskDuration, int[] TaskMilestone, int[][] TaskExper, int[][] TaskAdjacency)
        {
            this.TaskExper = TaskExper;
            this.TaskAdjacency = TaskAdjacency;
            this.TaskDuration = TaskDuration;
            this.TaskMilestone = TaskMilestone;

            this.Initial();

        }

        private void Initial()
        {

            // size of task
            NumOfTasks = this.TaskAdjacency.Length;

            // matrix task x start_finish
            TaskOfStartFinishTime = new int[this.NumOfTasks][];

            // add all start tasks(mean: task is not exited precedence task ~elements in vector all is 0)

            for (int i = 0; i < this.TaskAdjacency.Length; i++)
            {
                var vecAdj = this.TaskAdjacency[i];
                var isStartTask = true;
                foreach (var e in vecAdj)
                {
                    if (e == 1)
                    {
                        isStartTask = false;
                        break;

                    }

                }
                if (isStartTask)
                {
                    StartTasks.Add(i);
                }


            }


        }

        private bool isVailableWorkforce(int[] taskUnitTime, int[] workForcefUnitTime)
        {
            bool isAvailable = true;
            Parallel.ForEach(
                    Partitioner.Create(0, workForcefUnitTime.Length),
                    (range) =>
                    {
                        for (int i = range.Item1; i < range.Item2; i++)
                        {
                            int result = taskUnitTime[i] * workForcefUnitTime[i];
                            if (result > 0)
                            {
                                isAvailable = false;
                                break;
                            }
                        }
                    });

            return isAvailable;
        }


        private double mappingScore(int[] keySkills, int[] querySkills)

        // simarility score


        {

            int dotProduct = 0;
            double lengthSquared1 = 0;
            double lengthSquared2 = 0;



            for (int i = 0; i < querySkills.Length; i++)
            {
                dotProduct += querySkills[i] * keySkills[i];
                lengthSquared1 += Math.Pow(querySkills[i], 2);
                lengthSquared2 += Math.Pow(keySkills[i], 2);

            }


            double overallScore = dotProduct / (lengthSquared1 * lengthSquared2);

            // Nếu 2 thằng chỉ có 1 skill và giống nhau -> overallScore 
            return overallScore;
        }

        private List<int> getAvailableWorkforceIndexes(int[] taskUnitTime, List<int[]> assignedWorkforceOfUnitTime)
        {
            List<int> workforceIndexes = new List<int>();

            // Perform element-wise addition
            Parallel.ForEach(
                Partitioner.Create(0, assignedWorkforceOfUnitTime.Count),
                (range) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        bool isVailable = isVailableWorkforce(taskUnitTime, assignedWorkforceOfUnitTime[i]);
                        if (isVailable)
                        {
                            workforceIndexes.Add(i);
                        }
                    }

                });

            return workforceIndexes;
        }


        private int getBestWorkforceIndex(int[] requiredSkills, List<int> indexes, List<int[]> workforceOfSkill)
        {

            List<double> scores = Enumerable.Repeat(0.0, indexes.Count).ToList();

            // Perform element-wise addition
            Parallel.ForEach(
                Partitioner.Create(0, indexes.Count),
                (range) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        double score = mappingScore(requiredSkills, workforceOfSkill[indexes[i]]);
                        scores[i] = score;
                    }

                });


            double maxScore = scores.Max();

            if (maxScore == 0)
            {
                return -1;
            }

            int maxIndex = scores.IndexOf(maxScore);
            return indexes[maxIndex];
        }
        public int[] mergeHighSkill(int[] workforceSkills, int[] requiredSkills)
        {
            int[] mergedWorkforceSkills = new int[workforceSkills.Length];
            for (int i = 0; i < workforceSkills.Length; i++)
            {
                if (workforceSkills[i] < requiredSkills[i])
                {
                    mergedWorkforceSkills[i] = requiredSkills[i];
                }
                else
                {
                    mergedWorkforceSkills[i] = workforceSkills[i];
                }


            }
            return mergedWorkforceSkills;
        }



        private List<int[]> FitByMilestone(int MilestoneId)
        {


            List<int> SubTaskList = GroupedTaskMilestone[MilestoneId];

            List<int[]> AssignedWorkforceWithUnitTime = new List<int[]>();
            List<int[]> AssignedWorkforceOfSkill = new List<int[]>();
            List<List<int>> AssignedWorkforceOfTask = new List<List<int>>();

            Queue<int> queue = new Queue<int>();
            bool[] visited = new bool[SubTaskList.Count];


            try
            {
                // Them vao queue nhung task dau tien trong milestone
                for (int i = 0; i < SubTaskList.Count; i++)
                {
                    var _tIndex = SubTaskList[i];
                    bool isStartTask = true;
                    foreach (int j in SubTaskList)
                    {
                        if (TaskAdjacency[_tIndex][j] == 1)
                        {
                            isStartTask = false;
                            break;
                        }

                    }

                    if (isStartTask)
                    {
                        queue.Enqueue(i);
                    }

                }



                int count = 0;
                while (queue.Count > 0)
                {
                    count += 1;
                    if (count == 12)
                    {
                        int f = 1;
                    }
                    int v = queue.Dequeue();
                    if (visited[v] == false)
                    {
                        visited[v] = true;

                        int bestIndex = -1;
                        int[] UnitTimeWorkingTask = TaskSortedUnitTime[v];
                        if (AssignedWorkforceWithUnitTime.Count > 0)
                        {
                            // Kiểm tra xem những workforce chưa được assign trong khoảng [startTime, finishTime]
                            List<int> indexes = getAvailableWorkforceIndexes(UnitTimeWorkingTask, AssignedWorkforceWithUnitTime);


                            if (indexes.Count > 0)
                            {
                                // Tìm workforce có độ tương đồng cao nhất với điều kiện trùng lặp ít nhất 2 skills
                                bestIndex = getBestWorkforceIndex(TaskExper[SubTaskList[v]], indexes, AssignedWorkforceOfSkill);
                            }

                        }

                        // Nếu có thì update workForceOfUnitTime, workforceOfTasks, skillOfWorkforces
                        if (bestIndex != -1 & TaskDuration[SubTaskList[v]] != 0)
                        {
                            // Cập nhật workForceWithUnitTime
                            for (int i = 0; i < UnitTimeWorkingTask.Length; i++)
                            {
                                if (UnitTimeWorkingTask[i] == 1)
                                {
                                    AssignedWorkforceWithUnitTime[bestIndex][i] = 1;
                                }
                            }
                            // Cập nhật workforceOfTasks
                            AssignedWorkforceOfTask[bestIndex][SubTaskList[v]] = 1;
                            // Cập nhật skillOfWorkforces
                            AssignedWorkforceOfSkill[bestIndex] = this.mergeHighSkill(AssignedWorkforceOfSkill[bestIndex], TaskExper[SubTaskList[v]]);

                        }

                        // Nếu không có workforce nào thì tạo mới
                        if (bestIndex == -1 & TaskDuration[SubTaskList[v]] != 0)
                        {

                            // Thêm mới row assignedUnitTime vào workForceOfUnitTime cho một workforce mới
                            List<int> assignedTask = Enumerable.Repeat(0, this.NumOfTasks).ToList();

                            // Thêm mới row vào assignedUnitTime
                            AssignedWorkforceWithUnitTime.Add(UnitTimeWorkingTask);

                            // Thêm mới row vào workforceOfTasks
                            assignedTask[SubTaskList[v]] = 1;
                            AssignedWorkforceOfTask.Add(assignedTask);
                            AssignedWorkforceOfSkill.Add(this.TaskExper[SubTaskList[v]]);

                        }

                    }

                    // cuối cùng, thực hiện enque các node ở level tiếp theo         
                    for (int j = 0; j < SubTaskList.Count; j++)
                    {

                        if (this.TaskAdjacency[SubTaskList[j]][SubTaskList[v]] == 1 & queue.Contains(j) == false)
                        {
                            if (visited[j] == false)
                            {
                                queue.Enqueue(j);
                            }
                        }

                    }

                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


            return AssignedWorkforceOfSkill;
        }



        public Dictionary<int, List<int[]>> Fit()
        {

            Dictionary<int, List<int[]>> AssignedWorkforceByMilestone = new Dictionary<int, List<int[]>>();

            foreach (int milestoneId in this.GroupedTaskMilestone.Keys)
            {
                AssignedWorkforceByMilestone[milestoneId] = FitByMilestone(milestoneId);

            }
            //List<List<int>> assignedWorkforceOfTask = new List<List<int>>();
            //List<int[]> assignedWorkforceOfSkill = new List<int[]>();
            //List<int[]> assignedWorkforceOfUnitTime = new List<int[]>();
            //List<int[]> assignedWorkforceMilestone = new List<int[]>();

            //Queue<int> queue = new Queue<int>();
            //queue.Enqueue(START_TASK);

            //bool[] visited = new bool[NumOfTasks];

            //while (queue.Count > 0)
            //{
            //    int v = queue.Dequeue();

            //    if (visited[v] == false)
            //    {
            //        visited[v] = true;

            //        int bestIndex = -1;
            //        int[] taskUnitTime = TaskSortedUnitTime[v];
            //        if (assignedWorkforceOfUnitTime.Count > 0)
            //        {
            //            // Kiểm tra xem những workforce chưa được assign trong khoảng [startTime, finishTime]
            //            List<int> indexes = getAvailableWorkforceIndexes(taskUnitTime, assignedWorkforceOfUnitTime);


            //            if (indexes.Count > 0)
            //            {
            //                // Tìm workforce có độ tương đồng cao nhất với điều kiện trùng lặp ít nhất 2 skills
            //                bestIndex = getBestWorkforceIndex(TaskExper[v], indexes, assignedWorkforceOfSkill);
            //            }

            //        }

            //        // Nếu có thì update workForceOfUnitTime, workforceOfTasks, skillOfWorkforces
            //        if (bestIndex != -1 & TaskDuration[v] != 0)
            //        {
            //            // Cập nhật workForceOfUnitTime
            //            for (int i = 0; i < taskUnitTime.Length; i++)
            //            {
            //                if (taskUnitTime[i] == 1)
            //                {
            //                    assignedWorkforceOfUnitTime[bestIndex][i] = 1;
            //                }
            //            }
            //            // Cập nhật workforceOfTasks
            //            assignedWorkforceOfTask[bestIndex][v] = 1;
            //            // Cập nhật skillOfWorkforces
            //            assignedWorkforceOfSkill[bestIndex] = this.mergeHighSkill(assignedWorkforceOfSkill[bestIndex], TaskExper[v]);

            //        }

            //        // Nếu không có workforce nào thì tạo mới
            //        if (bestIndex == -1 & TaskDuration[v] != 0)
            //        {

            //            // Thêm mới row assignedUnitTime vào workForceOfUnitTime cho một workforce mới
            //            List<int> assignedTask = Enumerable.Repeat(0, this.NumOfTasks).ToList();

            //            // Thêm mới row vào assignedUnitTime
            //            assignedWorkforceOfUnitTime.Add(taskUnitTime);

            //            // Thêm mới row vào workforceOfTasks
            //            assignedTask[v] = 1;
            //            assignedWorkforceOfTask.Add(assignedTask);
            //            assignedWorkforceOfSkill.Add(this.TaskExper[v]);

            //        }

            //    }

            //    // cuối cùng, thực hiện enque các node ở level tiếp theo         
            //    for (int j = 0; j < this.TaskAdjacency[v].Length; j++)
            //    {
            //        if (this.TaskAdjacency[j][v] == 1 & queue.Contains(j) == false)
            //        {
            //            if (visited[j] == false)
            //            {
            //                queue.Enqueue(j);
            //            }
            //        }

            //    }



            //}


            return AssignedWorkforceByMilestone;

        }

        public void ForwardMethod()
        {

            GroupedTaskMilestone = new Dictionary<int, List<int>>();
            var unitTimes = new List<int>();


            // BFS
            bool[] visited = new bool[NumOfTasks];
            Queue<int> queue = new Queue<int>();


            // add all start tasks (mean: task is not exited precedence task ~ elements in vector all is 0)

            foreach (var i in StartTasks)
            {

                queue.Enqueue(i);

            }



            while (queue.Count > 0)
            {


                int v = queue.Dequeue();
                bool isVisitedAllPredencors = true;
                if (this.TaskOfStartFinishTime[v] == null)
                {
                    this.TaskOfStartFinishTime[v] = new int[2];
                }
                int ES = this.TaskOfStartFinishTime[v][0];
                int EF = this.TaskOfStartFinishTime[v][1];
                int duration = TaskDuration[v];


                // 1. kiểm tra xem các task trước v đã được duyệt chưa
                if (v != 0)
                {
                    for (int i = 0; i < NumOfTasks; ++i)
                    {
                        if (this.TaskAdjacency[v][i] == 1)
                        {
                            if (visited[i] == false)
                            {
                                isVisitedAllPredencors = false;
                                break;
                            }
                            else
                            {
                                if (ES < this.TaskOfStartFinishTime[i][1] + 1)
                                {
                                    ES = this.TaskOfStartFinishTime[i][1] + 1;
                                }
                            }
                        }
                    }

                }

                // nếu toàn bộ task trước v đã được duyệt
                // thêm task đó vào visited 
                // Cập nhật start time của task hiện tại =  finish time muộn nhất của Predencors + 1
                // Cập nhật finish time của task hiện tại =  start time + duration - 1
                if (isVisitedAllPredencors == true & visited[v] == false)
                {

                    visited[v] = true; // thêm task đó vào visited 

                    // Cập nhật EF = ES cho Start task và finish task 
                    if (duration == 0)
                    {
                        EF = ES; // Cập nhật early finish 
                    }
                    else
                    {
                        EF = ES + duration - 1; // Cập nhật early finish  
                    }

                    if (!unitTimes.Contains(EF))
                    {
                        unitTimes.Add(EF);
                    }

                    if (!unitTimes.Contains(ES))
                    {
                        unitTimes.Add(ES);
                    }

                    // Update Early start & Finish start
                    this.TaskOfStartFinishTime[v][0] = ES;
                    this.TaskOfStartFinishTime[v][1] = EF;


                    // Add task into group task by milestone
                    var milestoneId = this.TaskMilestone[v];

                    if (GroupedTaskMilestone.ContainsKey(milestoneId))
                    {
                        this.GroupedTaskMilestone[milestoneId].Add(v);
                    }
                    else
                    {
                        this.GroupedTaskMilestone[milestoneId] = new List<int>() { v };
                    }

                }



                // cuối cùng, thực hiện enque các node ở level tiếp theo
                for (int i = 0; i < this.TaskAdjacency[v].Length; i++)
                {
                    //if (v == 3 & i == 16) {
                    //    var d = 1;
                    //}
                    if (this.TaskAdjacency[i][v] == 1)
                    {
                        if (visited[i] == false & queue.Contains(i) == false)
                        {
                            queue.Enqueue(i);
                        }
                    }

                }

            }

            unitTimes.Sort();
            this.StortedUnitTimeList = unitTimes;


            // setup matrix task x unit time
            TaskSortedUnitTime = new int[this.NumOfTasks][];
            var check = 0;
            var check_j = 0;
            try
            {
                for (int i = 0; i < this.TaskAdjacency.Length; i++)
                {
                    //if(i == 4) {
                    //    check_j = 0;
                    //}
                    TaskSortedUnitTime[i] = new int[this.StortedUnitTimeList.Count];
                    for (int j = 0; j < this.StortedUnitTimeList.Count; j++)
                    {
                        check_j = j;
                        if (this.StortedUnitTimeList[j] >= this.TaskOfStartFinishTime[i][0] & this.StortedUnitTimeList[j] <= this.TaskOfStartFinishTime[i][1])
                        {
                            TaskSortedUnitTime[i][j] = 1;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ERRORsd");
            }

        }



    }
}