use vulkano::swapchain::{Swapchain, Surface, SurfaceTransform, PresentMode, ColorSpace, FullscreenExclusive};
use vulkano::image::ImageUsage;
use vulkano::device::Device;


pub struct Renderer<W>
{
    swapchain_: Swapchain<W>,
}

pub trait Render
{

}

impl Renderer<W>
{

}