
use ash::{vk};
use std::ffi::CStr;
use std::ptr;
use std::os::raw::c_void;
use ash::version::EntryV1_0;

pub struct ValidationInfo
{
    pub enabled_: bool,
    pub required_validation_layers_: [&'static str; 1],
}

unsafe extern "system" fn vulkan_debug_utils_callback(
    message_severity: vk::DebugUtilsMessageSeverityFlagsEXT,
    message_type: vk::DebugUtilsMessageTypeFlagsEXT,
    p_callback_data: *const vk::DebugUtilsMessengerCallbackDataEXT,
    _p_user_data: *mut c_void,
) -> vk::Bool32
{
    println!("Message");
    println!("{}", CStr::from_ptr((*p_callback_data).p_message).to_str().unwrap());

    vk::FALSE
}


pub fn check_validation_layer_support(entry: &ash::Entry, required_validation_layers: &Vec<&str>) -> bool
{
    if required_validation_layers.is_empty()
    {
       return true
    }

    let available_layers = entry.enumerate_instance_layer_properties().expect("Failed to enumerate instance layer properties");

    if available_layers.is_empty()
    {
        println!("No available layers");
        return false
    }

    for required_layer in required_validation_layers.iter()
    {
        let mut layer_found = false;

        for available_layer in available_layers.iter()
        {
            let available_layer_name = super::tools::c_char_array_to_string(&available_layer.layer_name);
            if(*required_layer) == available_layer_name
            {
                layer_found = true;
                break;
            }
        }

        if !layer_found
        {
            return false
        }
    }


    return true
}

pub fn setup_debug_utils(entry: &ash::Entry, instance: &ash::Instance) -> (ash::extensions::ext::DebugUtils, vk::DebugUtilsMessengerEXT)
{
    let debug_utils_loader = ash::extensions::ext::DebugUtils::new(entry, instance);

    let create_info = build_debug_messenger_create_info();

    let messenger = unsafe{
        debug_utils_loader.create_debug_utils_messenger(&create_info, None).expect("Could not create debug messenger")
    };
    (debug_utils_loader, messenger)
}

fn build_debug_messenger_create_info() -> vk::DebugUtilsMessengerCreateInfoEXT
{
    vk::DebugUtilsMessengerCreateInfoEXT {
        s_type: vk::StructureType::DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
        p_next: ptr::null(),
        flags: vk::DebugUtilsMessengerCreateFlagsEXT::empty(),
        message_severity: vk::DebugUtilsMessageSeverityFlagsEXT::WARNING |
             vk::DebugUtilsMessageSeverityFlagsEXT::VERBOSE |
             vk::DebugUtilsMessageSeverityFlagsEXT::INFO |
            vk::DebugUtilsMessageSeverityFlagsEXT::ERROR,
        message_type: vk::DebugUtilsMessageTypeFlagsEXT::GENERAL
            | vk::DebugUtilsMessageTypeFlagsEXT::PERFORMANCE
            | vk::DebugUtilsMessageTypeFlagsEXT::VALIDATION,
        pfn_user_callback: Some(vulkan_debug_utils_callback),
        p_user_data: ptr::null_mut(),
    }
}