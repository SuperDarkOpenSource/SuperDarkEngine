use super::debug::ValidationInfo;
use super::structures::DeviceExtension;

pub const VALIDATION: ValidationInfo = ValidationInfo{
    enabled_: true,
    required_validation_layers_: ["VK_LAYER_KHRONOS_validation"],
};

pub const DEVICE_EXTENSIONS: DeviceExtension = DeviceExtension{
    names: ["VK_KHR_swapchain"],
};