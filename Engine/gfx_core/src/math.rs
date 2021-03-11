use cgmath::{Vector3, Vector4, Matrix4, Matrix3, Quaternion};

type Mtype = f32;

pub type Vec3 = Vector3<Mtype>;
pub type Vec4 = Vector4<Mtype>;

pub type Mat3 = Matrix3<Mtype>;
pub type Mat4 = Matrix4<Mtype>;

pub type Quat = Quaternion<Mtype>;