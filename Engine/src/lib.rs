mod vulkan;

#[no_mangle]
pub extern fn super_dark_sum(a: u64, b: u64) -> u64 {
    a + b
}
