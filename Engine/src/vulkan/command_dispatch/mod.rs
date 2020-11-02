use vulkano::command_buffer::pool::{UnsafeCommandPool};
use vulkano::instance::{QueueFamily};
use vulkano::device::Device;
use array2d::Array2D;
use std::sync::Arc;

pub struct CommandDispatchCreateInfo<'a>
{
    pub num_threads_: u32,
    pub num_buffered_frames_: u32,
    pub graphics_queue_family_: QueueFamily<'a>,
}

pub struct CommandDispatch
{
    primary_graphics_pools_: Vec<Vec<UnsafeCommandPool>>,
}

impl CommandDispatch
{
    pub fn new(device: Arc<Device>, create_info: CommandDispatchCreateInfo) -> Option<CommandDispatch>
    {
        let mut primary_graphics_command_pools: Vec<Vec<UnsafeCommandPool>> = Vec::new();

        for thread in 1..=create_info.num_threads_
        {
            let mut row: Vec<UnsafeCommandPool> = Vec::new();
            for frame in 1..=create_info.num_buffered_frames_
            {
                row.push(UnsafeCommandPool::new(device.clone(), create_info.graphics_queue_family_, true, true).expect("Could not allocate command pool"));
            }
            primary_graphics_command_pools.push(row);
        }

        Some(CommandDispatch{primary_graphics_pools_: primary_graphics_command_pools})
    }
}