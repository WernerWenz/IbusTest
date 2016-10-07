using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ImeTest
{
	using IBusBus = IntPtr;

	using GDBusConnection = IntPtr;

	using IBusInputContext = IntPtr;

	using IBusEngineDesc = IntPtr;

    using IBusText = IntPtr;

    using IBusLookupTable = IntPtr;

    using GType = IntPtr;

    using GList = IntPtr;

    using GMainLoop = IntPtr;

    using GMainContext = IntPtr;


	public class GCDelegate: IDisposable
	{
		GCHandle _handle;
		IntPtr _funPtr;
		Delegate _d;

        public GCDelegate(Delegate d)
		{
            _d = d;
			_handle = GCHandle.Alloc (_d);
			_funPtr = Marshal.GetFunctionPointerForDelegate (_d);
		}

		public IntPtr Pointer { get { return _funPtr; } }

		public void Dispose()
		{
			_handle.Free ();
		}
	}

	public class Ibus: IDisposable
	{
        [DllImport("libX11")]
        public static extern uint XLookupKeysym(ref OpenTK.Platform.X11.XKeyEvent key_event, int index);
       

		const string LIB_IBUS = "libibus-1.0.so.5";

		const string LIB_GOBJECT = "libgobject-2.0.so";

		[DllImport(LIB_IBUS)]
		private static extern void ibus_init(); 

		[DllImport(LIB_IBUS)]
		private static extern IBusBus ibus_bus_new(); 

        [DllImport(LIB_IBUS)]
        private static extern void ibus_bus_set_watch_ibus_signal(IBusBus bus, bool watch); 

		[DllImport(LIB_IBUS)]
		private static extern bool ibus_bus_exit (IBusBus bus, bool restart);

		[DllImport(LIB_IBUS)]
		private static extern bool ibus_bus_is_connected (IBusBus bus);

		[DllImport(LIB_IBUS)]
		private static extern GDBusConnection ibus_bus_get_connection(IBusBus bus);

		//[DllImport(LIB_IBUS)]
		//private static extern UInt32 ibus_bus_request_name(IBusBus bus, StringBuilder name, UInt32 flags);

		[DllImport(LIB_IBUS)]
		private static extern IBusInputContext ibus_bus_create_input_context(IBusBus bus, string client_name);

		[DllImport(LIB_IBUS)]
		private static extern bool ibus_bus_set_global_engine(IBusBus bus, string global_engine);

        [DllImport(LIB_IBUS)]
        private static extern IBusEngineDesc ibus_bus_get_global_engine(IBusBus bus);

        [DllImport(LIB_IBUS)]
        private static extern GList ibus_bus_list_engines(IBusBus bus);

		[DllImport(LIB_IBUS)]
		private static extern void ibus_input_context_set_capabilities(IBusInputContext context, IBusCapabilite capabilities);

		[DllImport(LIB_IBUS)]
		private static extern bool ibus_input_context_process_key_event(IBusInputContext context, UInt32 keyval, UInt32 keycode, IBusModifierType state);

        [DllImport(LIB_IBUS)]
        private static extern void ibus_input_context_reset(IBusInputContext context);

		[DllImport(LIB_IBUS)]
		private static extern void ibus_input_context_focus_in(IBusInputContext context);

		[DllImport(LIB_IBUS)]
		private static extern void ibus_input_context_focus_out(IBusInputContext context);

		[DllImport(LIB_IBUS)]
		private static extern void ibus_input_context_set_cursor_location(IBusInputContext context, int x, int y, int width, int height);

		[DllImport(LIB_IBUS)]
		private static extern IBusEngineDesc ibus_input_context_get_engine(IBusInputContext context);

		[DllImport(LIB_IBUS)]
		private static extern void ibus_input_context_set_engine(IBusInputContext context, string name);

		[DllImport(LIB_IBUS)]
		private static extern string ibus_engine_desc_get_name(IBusEngineDesc info);

		[DllImport(LIB_IBUS)]
		private static extern string ibus_engine_desc_get_longname(IBusEngineDesc info);

        [DllImport(LIB_IBUS)]
        private static extern GType ibus_bus_get_type();

        [DllImport(LIB_IBUS)]
        private static extern GType ibus_input_context_get_type();

        [DllImport(LIB_IBUS)]
        private static extern string ibus_key_event_to_string(uint keyval, IBusModifierType modifiers);


        [DllImport(LIB_IBUS)]
        private static extern IntPtr ibus_text_get_text(IBusText text);


        [DllImport(LIB_IBUS)]
        private static extern uint ibus_lookup_table_get_number_of_candidates(IBusLookupTable table);

        [DllImport(LIB_IBUS)]
        private static extern IBusText ibus_lookup_table_get_candidate(IBusLookupTable table, uint index);

        [DllImport(LIB_IBUS)]
        private static extern uint ibus_lookup_table_get_cursor_pos(IBusLookupTable table);


        [DllImport(LIB_GOBJECT)]
		private static extern void g_signal_connect_data(IntPtr instance, string detailed_signal,  IntPtr c_handler, IntPtr data, IntPtr destroy_data, UInt32 connect_flags);

        [DllImport(LIB_GOBJECT)]
        private static extern GType g_type_from_name(string name);

        [DllImport(LIB_GOBJECT)]
        private static extern IntPtr g_signal_list_ids(GType typ, out int count);

        [DllImport(LIB_GOBJECT)]
        //[return: MarshalAs(UnmanagedType.LPStr)]
        private static extern IntPtr g_signal_name(uint id);

        [DllImport(LIB_GOBJECT)]
        private static extern uint g_list_length(GList list);

        [DllImport(LIB_GOBJECT)]
        private static extern IntPtr g_list_nth_data(GList list, uint n);

        [DllImport(LIB_GOBJECT)]
        private static extern GMainContext g_main_context_new();

        [DllImport(LIB_GOBJECT)]
        private static extern GMainLoop g_main_loop_new(GMainContext context, bool isRunning);

        [DllImport(LIB_GOBJECT)]
        private static extern void g_main_loop_run(GMainLoop loop);

        [DllImport(LIB_GOBJECT)]
        private static extern bool g_main_context_iteration(GMainContext ctx, bool blocking);


        //[DllImport(LIB_GOBJECT)]
        //private static extern void g_signal_emit_by_name(IntPtr instance, string detailed_signal, IntPtr user);

        private static void g_signal_connect(IntPtr instance, string detailed_signal,  IntPtr c_handler, IntPtr data)
        {
            g_signal_connect_data(instance, detailed_signal, c_handler, data, IntPtr.Zero, 0);
        }

        [DllImport(LIB_GOBJECT)]
        private static extern void g_free(IntPtr mem);

        [DllImport(LIB_GOBJECT)]
        private static extern void g_object_unref(IntPtr mem);
       


        public Action<string, int, bool> UpdatePreedit = delegate {};
        public Action<string, int, bool> UpdateAuxiliary = delegate {};
        public Action<string[], bool, uint> UpdateCandidates = delegate {};


		private IBusBus _bus;
		//private GDBusConnection _con;
		private IBusInputContext _ctx;

        private GMainContext _mainCtx;
        private GMainLoop _mainLoop;

        private GCDelegate _preeditCb;
        private GCDelegate _auxCb;
        private GCDelegate _updateLut;

        private string _startupEng;

		enum IBusCapabilite
		{
			PREEDIT_TEXT = 1 << 0,
			AUXILIARY_TEXT = 1 << 1,
			LOOKUP_TABLE = 1 << 2,
			FOCUS = 1 << 3,
			PROPERTY = 1 << 4,
			SURROUNDING_TEXT = 1 << 5
		}

		enum IBusModifierType
		{
			SHIFT_MASK = 1 << 0,
			LOCK_MASK = 1 << 1,
			CONTROL_MASK = 1 << 2,
            ALT_MASK = 1 << 3,
			MOD2_MASK = 1 << 4,
			MOD3_MASK = 1 << 5,
			MOD4_MASK = 1 << 6,
			MOD5_MASK = 1 << 7,
			BUTTON1_MASK = 1 << 8,
			BUTTON2_MASK = 1 << 9,
			BUTTON3_MASK = 1 << 10,
			BUTTON4_MASK = 1 << 11,
			BUTTON5_MASK = 1 << 12,

			HANDLED_MASK = 1 << 24,
			FORWARD_MASK = 1 << 25,
			IGNORED_MASK = 1 << 25,
			SUPER_MASK= 1 << 26,
			HYPER_MASK= 1 << 27,
			META_MASK= 1 << 28,
			RELEASE_MASK= 1 << 30,

			MODIFIER_MASK= 1 << 0x5c001fff
		}

        public void TestKey(OpenTK.Platform.X11.RawKeyEventArgs args, bool down, OpenTK.Input.KeyboardState ks)
		{
			IBusModifierType mod = 0;
			if (!down)
				mod |= IBusModifierType.RELEASE_MASK;

            if (ks.IsKeyDown(OpenTK.Input.Key.LShift) || ks.IsKeyDown(OpenTK.Input.Key.RShift))
                mod |= IBusModifierType.SHIFT_MASK;          
            if (ks.IsKeyDown(OpenTK.Input.Key.LAlt) || ks.IsKeyDown(OpenTK.Input.Key.RAlt))
                mod |= IBusModifierType.ALT_MASK;
            if (ks.IsKeyDown(OpenTK.Input.Key.LControl) || ks.IsKeyDown(OpenTK.Input.Key.RControl))
                mod |= IBusModifierType.CONTROL_MASK;


            uint keyval = XLookupKeysym(ref args.KeyEvent, 0);
            uint scancode = (uint)args.KeyEvent.keycode;

			var b = ibus_input_context_process_key_event(_ctx, keyval, scancode, mod);
            var hrs = ibus_key_event_to_string(keyval, mod);
            Console.WriteLine ("{0}: {1}, {2}　=> {3}", keyval, b, mod, hrs);

            Process();
		}

		public void TestSetFocus(bool focused)
		{
			if (focused)
			{
				Console.WriteLine ("Got Focus");
				ibus_input_context_focus_in (_ctx);               
			}
			else 
			{
				Console.WriteLine ("Lost Focus");
				ibus_input_context_focus_out (_ctx);
			}
		}

        public void TestCursor(int x, int y)
        {
            //Console.WriteLine ("Cursor => {0}, {1}", x, y);
            ibus_input_context_set_cursor_location(_ctx, x, y, 100, 16);
        }

        private void AddHandler(string name)
        {
            var cb = new GCDelegate ( new Action<IBusInputContext, IntPtr> ( (_ctx, data) =>
            {
                Console.WriteLine(name);
            }));
            g_signal_connect (_ctx, name, cb.Pointer, IntPtr.Zero);
        }

        private void DebugHandlers()
        {
            int numIds;
            // Warning: Leaking list
            var pids = g_signal_list_ids(ibus_input_context_get_type(), out numIds);
            for (int i = 0; i < numIds; i++)
            {
                var id = (uint)Marshal.ReadInt32(pids, 4 * i);
                var nameLP = g_signal_name(id);
                var name = Marshal.PtrToStringAnsi(nameLP);
                Console.WriteLine("{0} [{1}]: {2}", i, id, name);
                AddHandler(name);
            }
        }

        private static string StringFromUTF8(IntPtr utf8)
        {
            int len = 0;
            while (Marshal.ReadByte(utf8, len) != 0)
                len++;
            var mem = new byte[len];
            Marshal.Copy(utf8, mem, 0, len);
            return Encoding.UTF8.GetString(mem);
        }

		public Ibus ()
		{
            _mainCtx = g_main_context_new();
            _mainLoop = g_main_loop_new(_mainCtx, false);

			ibus_init();
			_bus = ibus_bus_new();

            ibus_bus_set_watch_ibus_signal(_bus, true);

            var engines = ibus_bus_list_engines(_bus);
            var numEng = g_list_length(engines);
            for (uint i = 0; i < numEng; i++)
            {
                var ptr = (IBusEngineDesc)g_list_nth_data(engines, i);
                var nam = ibus_engine_desc_get_name(ptr);
                Console.WriteLine("{0}: {1}", i, nam);
            }


            var cb = new GCDelegate ( new Action<IBusBus, IntPtr> ( (_ctx, data) =>
            {
                Console.WriteLine("global-engine-changed");
            }));
            g_signal_connect (_bus, "global-engine-changed", cb.Pointer, IntPtr.Zero);

			Console.WriteLine ("Connected: {0}", ibus_bus_is_connected (_bus));

			//_con = ibus_bus_get_connection (_bus);
			//var name = new StringBuilder (256);
			//ibus_bus_request_name (_bus, name, 0);

			_ctx = ibus_bus_create_input_context (_bus, "testapp");

            DebugHandlers();

            //ibus_input_context_set_capabilities(_ctx, IBusCapabilite.FOCUS);
            //ibus_input_context_set_capabilities(_ctx, (IBusCapabilite)7);
            ibus_input_context_set_capabilities(_ctx, (IBusCapabilite)0x7FFFFFF);
            //ibus_input_context_set_capabilities(_ctx, IBusCapabilite.PREEDIT_TEXT | IBusCapabilite.FOCUS | IBusCapabilite.AUXILIARY_TEXT);
            ibus_input_context_focus_in (_ctx);

            var gdesc = ibus_bus_get_global_engine(_bus);
            _startupEng = ibus_engine_desc_get_name(gdesc);

			//ibus_bus_set_global_engine (_bus, "anthy");
			ibus_input_context_set_engine (_ctx, "anthy");

			var desc = ibus_input_context_get_engine(_ctx);
            var name = ibus_engine_desc_get_name(desc);
            Console.WriteLine("Engine: {0}", name);
			Console.WriteLine ("Ctx: #{0}", _ctx);

            _preeditCb = new GCDelegate ( new Action<IBusInputContext, IBusText, int, bool> ( (_ctx, _text, _pos, _vis) =>
            {
                var text = StringFromUTF8(ibus_text_get_text(_text));
                UpdatePreedit(text, _pos, _vis);
            }));
            g_signal_connect (_ctx, "update-preedit-text", _preeditCb.Pointer, IntPtr.Zero);

            _auxCb = new GCDelegate ( new Action<IBusInputContext, IBusText, int, bool> ( (_ctx, _text, _pos, _vis) =>
            {
                var text = StringFromUTF8(ibus_text_get_text(_text));
                UpdateAuxiliary(text, _pos, _vis);
            }));
            g_signal_connect (_ctx, "update-auxiliary-text", _auxCb.Pointer, IntPtr.Zero);

            _updateLut = new GCDelegate ( new Action<IBusInputContext, IBusLookupTable, bool> ( (_ctx, _tab, _vis) =>
            {
                uint numCand = ibus_lookup_table_get_number_of_candidates(_tab);
                var cands = new string[numCand];
                for (uint i = 0; i < numCand; i++)
                    cands[i] = StringFromUTF8(ibus_text_get_text(ibus_lookup_table_get_candidate(_tab, i)));
                uint selected = ibus_lookup_table_get_cursor_pos(_tab);
                UpdateCandidates(cands, _vis, selected);
            }));
            g_signal_connect (_ctx, "update-lookup-table", _updateLut.Pointer, IntPtr.Zero);


		}

        public void Run()
        {
            
            
        }

        public void Process()
        {
            while (g_main_context_iteration(IntPtr.Zero, false))
            {
            }
        }

		public void Dispose()
		{
            if (_ctx != IntPtr.Zero)
            {
                ibus_input_context_set_engine(_ctx, _startupEng);

                ibus_input_context_focus_out(_ctx);
                ibus_input_context_reset(_ctx);
                //g_object_unref(_ctx);
                _ctx = IntPtr.Zero;

    			//ibus_bus_exit (_bus, false);
            }
		}
	}
}

