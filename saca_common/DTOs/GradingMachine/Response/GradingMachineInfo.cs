using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.GradingMachine.Response
{
    public class GradingMachineInfo
    {
        //version
        public bool is_active { get; set; } 
        public string version { get; set; } = null!;

        //Worker Info
        public double queue_size { get; set; }
        public double worker_available { get; set; }
        public double worker_idle { get; set; } 
        public double worker_working { get; set; }
        public double worker_pause { get; set; }
        public double job_failed { get; set; }

        //Config Info
        public bool enable_wait_result { get; set; }
        public bool enable_compiler_options { get; set; }
        public bool enable_command_line_arguments { get; set; }
        public bool enable_submission_delete { get; set; }
        public double max_queue_size { get; set; }
        public double cpu_time_limit { get; set; }
        public double max_cpu_time_limit { get; set; }
        public double cpu_extra_time { get; set; }
        public double max_cpu_extra_time { get; set; }
        public double wall_time_limit { get; set; }
        public double max_wall_time_limit { get; set; }
        public double memory_limit { get; set; }
        public double max_memory_limit { get; set; }
        public double stack_limit { get; set; }
        public double max_stack_limit { get; set; }
        public double max_processes_and_or_threads { get; set; }
        public double max_max_processes_and_or_threads { get; set; }
        public bool enable_per_process_and_thread_time_limit { get; set; }
        public bool allow_enable_per_process_and_thread_time_limit { get; set; }
        public bool enable_per_process_and_thread_memory_limit { get; set; }
        public bool allow_enable_per_process_and_thread_memory_limit { get; set; }
        public double max_file_size { get; set; }
        public double max_max_file_size { get; set; }
        public double number_of_runs { get; set; }
        public double max_number_of_runs { get; set; }

    }
}
