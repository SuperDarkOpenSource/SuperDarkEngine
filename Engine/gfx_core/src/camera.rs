use crate::math::{Vec3, Quat};

pub struct Camera
{
    pub position: Vec3,
    pub orientation: Quat,
    pub ry: f32,
}

impl Camera
{
    pub fn new(position: Vec3, orientation: Quat, ry: f32) -> Self
    {
        Camera
        {
            position,
            orientation,
            ry
        }
    }
}