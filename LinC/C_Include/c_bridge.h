#pragma pack(1)
typedef struct vars {
    int num_vars;
    void** vars;
} vars_t;
#pragma pack()
#ifdef __cplusplus
extern "C" {
#endif
#ifdef _WIN32
#  ifdef MODULE_API_EXPORTS
#    define MODULE_API __declspec(dllexport)
#  else
#    define MODULE_API __declspec(dllimport)
#  endif
#else
#  define MODULE_API
#endif
MODULE_API void bridge_init();
MODULE_API void receive_data(void* data);
#ifdef __cplusplus
}
#endif
