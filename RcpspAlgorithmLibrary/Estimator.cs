using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections;

namespace Monster
{
    public class ScheduleEstimator
    {
        const int START_TASK = 0;
        public int NumOfTasks = 0;
        
        public List<int> TaskDuration = new List<int>();
        public List<List<int>> TaskAdjacency = new List<List<int>>();
        public List<List<int>> TaskExper = new List<List<int>>();

        public List<List<int>> TaskOfStartFinishTime =  new List<List<int>>();
        public List<List<int>> TaskSortedUnitTime = new List<List<int>>();
        public List<int> StortedUnitTimeList = new List<int>();
    
        public ScheduleEstimator(List<int> TaskDuration, List<List<int>> TaskExper, List<List<int>> TaskAdjacency){
            this.TaskExper = TaskExper;
            this.TaskAdjacency = TaskAdjacency;
            this.TaskDuration = TaskDuration;
            this.Initial();

        }

        private void Initial(){

            // size of task
            NumOfTasks = this.TaskAdjacency[0].Count;

            // matrix task x start_finish
            TaskOfStartFinishTime  = Enumerable.Repeat(new List<int>(), this.NumOfTasks).Select(x => Enumerable.Repeat(0, 2).ToList()).ToList();

        }

        private bool isVailableWorkforce(List<int> taskUnitTime, List<int> workForcefUnitTime){
            bool isAvailable = true;
            Parallel.ForEach(
                    Partitioner.Create(0, workForcefUnitTime.Count),
                    (range) =>
                    {
                        for (int i = range.Item1; i < range.Item2; i++)
                        {
                            int result = taskUnitTime[i] * workForcefUnitTime[i];
                            if (result > 0){
                                isAvailable = false;
                                break;
                            }
                        }
                    });

            return isAvailable;
        }


        private int mappingScore(List<int> keySkills, List<int> querySkills){
            int overallScore = 0;
            for (int i =0; i < querySkills.Count; i++)
            {
                if (keySkills[i] > 0 & querySkills[i] > 0){
                    overallScore += 1;
                }
                
            }

            return overallScore;
        }

        private List<int> getAvailableWorkforceIndexes(List<int> taskUnitTime, List<List<int>> assignedWorkForceOfUnitTime){
            List<int> workforceIndexes = new List<int>();

            // Perform element-wise addition
            Parallel.ForEach(
                Partitioner.Create(0, assignedWorkForceOfUnitTime.Count),
                (range) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        bool isVailable = isVailableWorkforce(taskUnitTime, assignedWorkForceOfUnitTime[i]);
                        if (isVailable){
                            workforceIndexes.Add(i);
                        }
                    }
                    
                });

            return workforceIndexes;
        }


        private int getBestWorkforceIndex(List<int> requiredSkills, List<int> indexes,  List<List<int>> workforceOfSkill){
            
            List<int> scores = Enumerable.Repeat(0, indexes.Count).ToList();

            // Perform element-wise addition
            Parallel.ForEach(
                Partitioner.Create(0, indexes.Count),
                (range) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        int score = mappingScore(requiredSkills, workforceOfSkill[indexes[i]]);
                        scores[i]  = score;
                    }
                    
                });

            
            int maxScore = scores.Max();
            
            if (maxScore < 2){
                return -1;
            }
            
            int maxIndex = scores.IndexOf(maxScore);
            return indexes[maxIndex];
        }
        public List<int> mergeHighSkill(List<int> workforceSkills, List<int> requiredSkills){
            List<int> mergedWorkforceSkills = Enumerable.Repeat(0, workforceSkills.Count).ToList();
            for (int i = 0; i < workforceSkills.Count; i ++){
                if (workforceSkills[i] < requiredSkills[i]){
                    mergedWorkforceSkills[i] = requiredSkills[i];
                }
                else{
                    mergedWorkforceSkills[i]  = workforceSkills[i];
                }


            }
            return mergedWorkforceSkills;
        }

        public List<List<int>> Fit(){
            
            List<List<int>> assignedWorkforceOfTask = new List<List<int>>(); 
            List<List<int>> assignedWorkforceOfSkill = new List<List<int>>(); 
            List<List<int>> assignedWorkForceOfUnitTime = new List<List<int>>(); 
            
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(START_TASK);

            bool[] visited = new bool[NumOfTasks];

            while (queue.Count > 0)
            {
                int v = queue.Dequeue();

                if (visited[v] == false){
                    visited[v] = true; 
                    
                    int bestIndex = -1;
                    List<int> taskUnitTime = TaskSortedUnitTime[v];
                    if (assignedWorkForceOfUnitTime.Count > 0){
                        // Kiểm tra xem những workforce chưa được assign trong khoảng [startTime, finishTime]
                        List<int> indexes = getAvailableWorkforceIndexes(taskUnitTime, assignedWorkForceOfUnitTime);

                        
                        if (indexes.Count > 0){
                            // Tìm workforce có độ tương đồng cao nhất với điều kiện trùng lặp ít nhất 2 skills
                            bestIndex = getBestWorkforceIndex(TaskExper[v], indexes, assignedWorkforceOfSkill);
                        }

                    }
                    
                    // Nếu có thì update workForceOfUnitTime, workforceOfTasks, skillOfWorkforces
                    if (bestIndex != -1){
                        // Cập nhật workForceOfUnitTime
                        for (int i = 0; i < taskUnitTime.Count; i++){
                            if (taskUnitTime[i] == 1){
                                assignedWorkForceOfUnitTime[bestIndex][i] = 1;
                            }
                        }
                        // Cập nhật workforceOfTasks
                        assignedWorkforceOfTask[bestIndex][v] = 1;
                        // Cập nhật skillOfWorkforces
                        assignedWorkforceOfSkill[bestIndex] = this.mergeHighSkill(assignedWorkforceOfSkill[bestIndex], TaskExper[v]);

                    }

                    // Nếu không có workforce nào thì tạo mới
                    if (bestIndex == -1){

                        // Thêm mới row assignedUnitTime vào workForceOfUnitTime cho một workforce mới
                        List<int> assignedUnitTime = Enumerable.Repeat(0, this.StortedUnitTimeList.Count).ToList();
                        List<int> assignedTask = Enumerable.Repeat(0, this.NumOfTasks).ToList();
                        List<int> skillWorkforce = Enumerable.Repeat(0, this.TaskExper[0].Count).ToList();
                        
                        // Thêm mới row vào assignedUnitTime
                        assignedWorkForceOfUnitTime.Add(taskUnitTime);
                        
                        // Thêm mới row vào workforceOfTasks
                        assignedTask[v] = 1;
                        assignedWorkforceOfTask.Add(assignedTask);
                        assignedWorkforceOfSkill.Add(this.TaskExper[v]);

                    }

                }

                // cuối cùng, thực hiện enque các node ở level tiếp theo         
                for (int j = 0; j < this.TaskAdjacency[v].Count; j++){
                    if (this.TaskAdjacency[j][v] == 1  & queue.Contains(j) == false){
                        if (visited[j] == false){
                            queue.Enqueue(j);
                        }  
                    }
                    
                }  
                
            }

            for (int i = 0; i < assignedWorkforceOfSkill.Count; i ++){
                Console.WriteLine("[" + string.Join(",", assignedWorkforceOfSkill[i]) + "]");
            }

            return assignedWorkforceOfSkill;
            
        }

        public void ForwardMethod(){

            List<int> unitTimes = new List<int>();
            
            // BFS
            bool[] visited = new bool[NumOfTasks];
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(START_TASK);
            

            while (queue.Count > 0){
                int v = queue.Dequeue();
                bool isVisitedAllPredencors = true;
                int ES = this.TaskOfStartFinishTime[v][0];
                int EF = this.TaskOfStartFinishTime[v][1];
                int duration =  TaskDuration[v];
                
                
                // 1. kiểm tra xem các task trước v đã được duyệt chưa
                if (v != 0){
                    for (int i = 0; i < NumOfTasks; ++i){
                        if ( this.TaskAdjacency[v][i] == 1){                 
                            if (visited[i] == false){
                                isVisitedAllPredencors = false;
                                break;
                            }
                            else{
                                if (ES <  this.TaskOfStartFinishTime[i][1] + 1){
                                    ES =  this.TaskOfStartFinishTime[i][1] + 1;
                                }
                            }
                        }
                    }
                
                }

                // nếu toàn bộ task trước v đã được duyệt
                // thêm task đó vào visited 
                // Cập nhật start time của task hiện tại =  finish time muộn nhất của Predencors + 1
                // Cập nhật finish time của task hiện tại =  start time + duration - 1
                if (isVisitedAllPredencors == true & visited[v] == false){
                    if (v != 0){
                        // here
                    }
                    visited[v] = true; // thêm task đó vào visited 
           
                    // if (v != START_TASK){
                    EF = ES + duration; 
                    // Cập nhật EF = ES cho Start task và finish task 
                    if (duration == 0){
                        EF = ES; // Cập nhật early finish 
                    }
                    else{
                        EF = ES + duration - 1; // Cập nhật early finish  
                    }
                    // }

                    if (!unitTimes.Contains(EF))
                    {
                         unitTimes.Add(EF);
                    }

                    if (!unitTimes.Contains(ES))
                    {
                         unitTimes.Add(ES);
                    }
                }

                // Update
                this.TaskOfStartFinishTime[v][0] = ES;
                this.TaskOfStartFinishTime[v][1] = EF;

                // cuối cùng, thực hiện enque các node ở level tiếp theo
                for (int i = 0; i <  this.TaskAdjacency[v].Count; i++){
                    if (  this.TaskAdjacency[i][v] == 1){
                        if (visited[i] == false & queue.Contains(i) == false){
                    
                            queue.Enqueue(i);
                            

                            
                        }  
                    }
                    
                }
                
            }

            unitTimes.Sort();
            this.StortedUnitTimeList = unitTimes;
            
            // setup matrix task x unit time
            setupTaskOfUnitTime();
        }

        private void setupTaskOfUnitTime(){

            // matrix task x start_finish
            TaskSortedUnitTime  = Enumerable.Repeat(new List<int>(), this.NumOfTasks).Select(x => Enumerable.Repeat(0, this.StortedUnitTimeList.Count).ToList()).ToList();

            for (int i = 0; i < this.TaskAdjacency.Count; i++){
                for (int j = 0; j < this.StortedUnitTimeList.Count; j++){
                    if (this.StortedUnitTimeList[j] >=this.TaskOfStartFinishTime[i][0] & this.StortedUnitTimeList[j] <= this.TaskOfStartFinishTime[i][1]){
                        TaskSortedUnitTime[i][j] = 1;
                    }
                    
                }
            }
        }

    }
}
