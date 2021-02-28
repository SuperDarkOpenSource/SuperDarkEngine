use ash::vk;
use super::utility::tools;
use ash::version::InstanceV1_0;
/// Stores information on the preferred device queue family indices for each operation type
struct QueueFamilyIndices
{
    graphics_family_: Option<u32>,
    graphics_compute_family: Option<u32>,
    compute_family: Option<u32>,
    transfer_family: Option<u32>
}

/// Contains capabilities, preferred presentation attributes, and the vulkan handle for a given
/// physical device. This allows physical devices and device capabilities to be enumerated and
/// selected by an application at runtime
pub struct PhysicalDeviceInfo
{
    pub physical_device_: vk::PhysicalDevice,
    pub device_name_: String,
}

impl PhysicalDeviceInfo
{
    /// Creates a new PhysicalDeviceInfo containing physical device information, as well as the
    /// device's vulkan handle
    ///
    /// # Arguments
    ///
    /// * 'instance' - a reference to the vulkan instance, used to enumerate device capabilities
    ///
    /// * 'physical_device' - the vulkan physical device handle
    ///
    pub fn new(instance: &ash::Instance, physical_device: vk::PhysicalDevice) -> PhysicalDeviceInfo
    {
        let device_properties = unsafe {instance.get_physical_device_properties(physical_device)};
        let device_name = tools::c_char_array_to_string(&device_properties.device_name);
        PhysicalDeviceInfo{physical_device_: physical_device, device_name_: device_name}
    }

}

/// Returns a list of PhysicalDeviceInfo representing physical devices capable of rendering. A
/// suitable device must be a GPU capable of presenting to given surface types, that
/// supports all device extensions required by the application
///
/// # Arguments
///
/// * 'instance' - a reference to the vulkan instance, used to enumerate physical devices and device
/// capabilities
///
/// * 'required_extensions' - a list of device extensions required by the application
///
pub fn get_suitable_physical_devices(instance: &ash::Instance, required_extensions: &Vec<&str>) -> Vec<PhysicalDeviceInfo>
{
    let physical_devices = unsafe {instance.enumerate_physical_devices().expect("Could not enumerate physical devices")};
    let mut suitable_devices: Vec<PhysicalDeviceInfo> = Vec::new();
    for device in physical_devices
    {
        if is_suitable(instance, device, required_extensions)
        {
            suitable_devices.push(PhysicalDeviceInfo::new(instance, device));
        }

    }

    suitable_devices
}

/// Returns true if a given physical device is a GPU
///
/// # Arguments
///
/// * 'instance' - a reference to the vulkan instance, used to enumerate device properties
///
/// * 'physical_device' - the vulkan physical device handle
///
fn is_gpu(instance: &ash::Instance, physical_device: vk::PhysicalDevice) -> bool
{
    let device_properties = unsafe {instance.get_physical_device_properties(physical_device)};

    match device_properties.device_type
    {

        vk::PhysicalDeviceType::DISCRETE_GPU | vk::PhysicalDeviceType::INTEGRATED_GPU | vk::PhysicalDeviceType::VIRTUAL_GPU =>{
            true
        },
        _ => false
    }
}

/// Returns true if a given physical device supports the requested device extensions
///
/// # Arguments
///
/// * 'instance' - a reference to the vulkan instance, used to enumerate supported device extensions
///
/// * 'physical_device' - the vulkan physical device handle
///
fn supports_extensions(instance: &ash::Instance, physical_device: vk::PhysicalDevice,  required_extensions: &Vec<&str>) -> bool
{
    let device_extensions = unsafe{instance.enumerate_device_extension_properties(physical_device).expect("Could not enumeration device extension properties")};

    for requested_extension in required_extensions
    {

        if !device_extensions.iter().any(|&i| tools::c_char_array_to_string(&i.extension_name) == requested_extension.to_string())
        {
            return false;
        }
    }
    true
}

/// Returns true if a given physical device supports graphics and presentation
///
/// # Arguments
///
/// * 'instance' - a reference to the vulkan instance, used to enumerate queue family properties
///
/// * 'physical_device' - the vulkan physical device handle
///
fn supports_graphics_and_presentation(instance: &ash::Instance, physical_device: vk::PhysicalDevice) -> bool
{
    let device_queue_families = unsafe{instance.get_physical_device_queue_family_properties(physical_device)};

    device_queue_families.iter().any(|&i| i.queue_flags.contains(vk::QueueFlags::GRAPHICS))
}

/// Determines whether a physical device is suitable for the engine and application. A device is
/// suitable if it is a GPU that supports graphics and presentation and all requested device
/// extensions.
///
/// # Arguments
///
/// * 'instance' - a reference to the vulkan instance, used to enumerate device properties
///
/// * 'physical_device' - the vulkan physical device handle
///
/// * 'required_extensions' - a list of device extensions required by the engine and application
///
fn is_suitable(instance: &ash::Instance, physical_device: vk::PhysicalDevice, required_extensions: &Vec<&str>) -> bool
{
        return is_gpu(instance, physical_device) && supports_graphics_and_presentation(instance, physical_device) && supports_extensions(instance, physical_device, required_extensions)
}


