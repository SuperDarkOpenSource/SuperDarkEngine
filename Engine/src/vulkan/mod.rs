mod utility;
mod physical_device;

use ash::{vk};
use ash::version::{EntryV1_0};
use ash::vk::make_version;

use std::ffi::CString;
use std::ptr;

use utility::constants;
use utility::debug;

/// Used to configure a vulkan::Engine on Engine creation
pub struct EngineCreateInfo
{
    /// The name of the application using this engine
    pub app_name_: CString,
    /// The version of the application using this engine
    pub app_version_: u32,
    /// Whether vulkan validation layers should be enabled for debugging purposes
    pub validation_enabled_: bool,
}

/// Manages all aspects of the vulkan context, and contains functionality for rendering to multiple
/// windows. Creating an Engine will initialize the vulkan context. Destroying an Engine will delete
/// all vulkan resources, but will not destroy any windows or external geometry data.
pub struct Engine
{
    /// vulkan function loader
    entry_: ash::Entry,
    /// vulkan instance, configured to support requested validation layers and instance extensions
    instance_: ash::Instance,
    /// optional validation layers and debug messenger for handling validation messages
    debug_: Option<(ash::extensions::ext::DebugUtils, vk::DebugUtilsMessengerEXT)>,
}

impl Engine
{
    /// Creates a new vulkan engine, configured to support requested validation layers and instance
    /// extensions. Panics if any part of Engine creation fails.
    ///
    /// # Arguments
    ///
    /// * 'create_info' - contains all data necessary to configure the Engine
    ///
    pub fn new(create_info: &EngineCreateInfo) -> Engine
    {
        // Load Vulkan entry functions. Panic if vulkan functions cannot be loaded
        let entry = ash::Entry::new().expect("Could not load vulkan functions");
        // Configure and create the vulkan instance. Panic if instance cannot be created
        let instance = Engine::create_instance(&entry, &create_info);
        // Enable validation if requested. Panic if the debug messenger cannot be created
        let debug = Engine::enable_validation(create_info.validation_enabled_, &entry, &instance);
        // Get a vector of suitable physical devices. Panic if devices or device properties cannot
        // be enumerated
        let physical_devices = physical_device::get_suitable_physical_devices(&instance, &constants::DEVICE_EXTENSIONS.names.to_vec());

        Engine{entry_: entry, instance_: instance, debug_:debug}
    }

    /// Creates a vulkan instance configured with requested extension and validation support
    ///
    /// # Arguments
    ///
    /// * 'entry' - vulkan function loader used to create the instance
    ///
    /// * 'create_info' - contains all data necessary to configure the Engine
    ///
    fn create_instance(entry: &ash::Entry, create_info: &EngineCreateInfo) -> ash::Instance
    {
        let application_info = vk::ApplicationInfo
        {
            s_type: vk::StructureType::APPLICATION_INFO,
            p_next: ptr::null(),
            p_application_name: create_info.app_name_.as_ptr(),
            application_version: create_info.app_version_,
            p_engine_name: CString::new(env!("CARGO_PKG_NAME")).expect("Could not find cargo package name").as_ptr(),
            engine_version: make_version(1, 0, 0),
            api_version: make_version(1, 0, 0),
        };

        // TODO: Move instance extensions to EngineCreateInfo
        let extension_names = utility::platforms::required_extension_names();

        let create_info = vk::InstanceCreateInfo
        {
            s_type: vk::StructureType::INSTANCE_CREATE_INFO,
            p_next: ptr::null(),
            flags: vk::InstanceCreateFlags::empty(),
            p_application_info: &application_info,
            pp_enabled_layer_names: ptr::null(),
            enabled_layer_count: 0,
            pp_enabled_extension_names: extension_names.as_ptr(),
            enabled_extension_count: extension_names.len() as u32,
        };

        unsafe {entry.create_instance(&create_info, None).expect("Failed to create instance")}
    }

    /// Enables validation layers if validation is requested. Fails silently if validation is not
    /// requested, or if the instance does not support the requested validation layers
    ///
    /// # Arguments
    ///
    /// * 'validation_enabled' - whether validation layers should be enabled
    ///
    /// * 'instance' - the vulkan instance validation will be enabled for
    ///
    fn enable_validation(validation_enabled: bool, entry: &ash::Entry, instance: &ash::Instance) -> Option<(ash::extensions::ext::DebugUtils, vk::DebugUtilsMessengerEXT)>
    {
        if validation_enabled &&
            debug::check_validation_layer_support(&entry, &constants::VALIDATION.required_validation_layers_.to_vec())
        {
            Some(debug::setup_debug_utils(entry, instance))
        }
        else
        {

            None
        }
    }
}

#[cfg(test)]
mod tests
{

    #[test]
    fn engine_creation_test()
    {
        let create_info = super::EngineCreateInfo{app_name_: super::CString::new("Hello").expect("Could not create app name"),
            app_version_: 1,
            validation_enabled_: true};

        let engine = super::Engine::new(&create_info);

    }


}
