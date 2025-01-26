namespace WillPittenger.Goodies.Sounds;

/// <summary>
/// Declares <see cref="ISound"/> instances for sounds common to recent Windows versions.
/// </summary>
/// <remarks>
/// You use <see cref="SoundIDs"/> values to specify a sound.  Use <see cref="AllPredefinedSounds"/> to obtain the instance of <see cref="ISound"/>.  In some
/// cases, this instance will be a <see cref="SoundFile"/> while in five cases, it's a <see cref="SysSound"/> instance.  See the notes for <see
/// cref="SoundIDs"/> for more details.
/// </remarks>
public static class PredefinedSounds
{
	#region Constructors
	#endregion

	#region Delegates
	#endregion

	#region Events
	#endregion

	#region Constants
	#endregion

	#region Helper Types
		/// <summary>
		/// Declares the ID values you'll use to obtain a predefined sound.
		/// </summary>
		/// <remarks>
		/// <para>The first 5 listed are all instances of <see cref="SysSound"/> and wrap a <see cref="System.Media.SystemSound"/> instance.  That class has
		/// limited functionality so <see cref="ISound.PlayLooping"/> and <see cref="ISound.PlaySync"/> are treated as simple <see cref="ISound.Play"/> calls.
		/// </para>
		/// 
		/// <para>The rest map to instances of <see cref="SoundFile"/>.  When your code creates a <see cref="SoundFile"/> instance, the file must exist.  However,
		/// it can't be assumed that the predefined files are on every single machine.  It's assumed, if these files are present, they'll be inside
		/// <c>C:\Windows\Media\</c>.  Playback will fail if they're missing.</para>
		/// </remarks>
		public enum SoundIDs
		{
			/// <summary>
			/// This is the standard asterisk sound associated with message boxes on Windows.  It is always available, but playback functionality is limited.
			/// </summary>
			asterisk,

			/// <summary>
			/// This is a basic beep sound.  It is always available, but playback functionality is limited.
			/// </summary>
			beep,

			/// <summary>
			/// This is a standard sound associated with warning message boxes.  It is always available, but playback functionality is limited.
			/// </summary>
			exclamation,

			/// <summary>
			/// This is a standard sound associated with error message boxes.  It is always available, but playback functionality is limited.
			/// </summary>
			hand,

			/// <summary>
			/// This is a standard sound associated with question message boxes.  It is always available, but playback functionality is limited.
			/// </summary>
			question,


			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm01,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm02,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm03,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm04,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm05,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm06,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm07,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm08,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm09,

			/// <summary>
			/// This is one of 10 alarm sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			alarm10,

			/// <summary>
			/// This is a standard chimes sound.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			chimes,

			/// <summary>
			/// This is a standard chords sound.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			chord,

			/// <summary>
			/// This is a standard "ding" or bell sound.  It dates back to the 1990s and should be available on any modern version of Windows.
			/// </summary>
			ding,

			/// <summary>
			/// This is a standard chords sound.  It's unknown how many PCs have this sound.  It's a <see cref="SoundFile"/>.
			/// </summary>
			flourish,

			/// <summary>
			/// This is a standard notification sound.  It dates back to the 1990s and should be available on any modern version of Windows.
			/// </summary>
			notify,

			/// <summary>
			/// It's unknown what this sound represents.  It's unknown how many PCs have this sound.  It's a <see cref="SoundFile"/>.
			/// </summary>
			oneStop,

			/// <summary>
			/// This is a standard file recycle sound.  It dates back to the 1990s and should be available on any modern version of Windows.
			/// </summary>
			recycle,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring01,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring02,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring03,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring04,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring05,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring06,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring07,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring08,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring09,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ring10,

			/// <summary>
			/// This is one of 11 phone ringing sounds.  Most of the time this will be available.  It's a <see cref="SoundFile"/>.
			/// </summary>
			ringOut,

			/// <summary>
			/// Exact purpose is unknown, but is probably something to do with speech.  It might not exist in versions of Windows older than Windows 10.
			/// </summary>
			speechDisambig,

			/// <summary>
			/// Exact purpose is unknown, but is probably something to do with speech.  It might not exist in versions of Windows older than Windows 10.
			/// </summary>
			speechMisrecog,

			/// <summary>
			/// Exact purpose is unknown, but is probably something to do with speech.  It might not exist in versions of Windows older than Windows 10.
			/// </summary>
			speechOff,

			/// <summary>
			/// Exact purpose is unknown, but is probably something to do with speech.  It might not exist in versions of Windows older than Windows 10.
			/// </summary>
			speechOn,

			/// <summary>
			/// Exact purpose is unknown, but is probably something to do with speech.  It might not exist in versions of Windows older than Windows 10.
			/// </summary>
			speechSleep,

			/// <summary>
			/// This is a standard sound dating to the 1990s to be played when a task, like copying files, is complete.  It should be present on all machines.
			/// </summary>
			tada,

			/// <summary>
			/// It's unknown what this sound represents.  It's unknown how many PCs have this sound.  It's a <see cref="SoundFile"/>.
			/// </summary>
			town,

			/// <summary>
			/// This sound has something to do with backgrounds, but details are unknown as well as how well you can expect this sound to be available.
			/// </summary>
			windowsBkg,

			/// <summary>
			/// Played when Windows displays a balloon-style tooltip.  (Maybe???)  It's doubtful this is any older than those balloon-style tooltips.
			/// </summary>
			windowsBalloon,

			/// <summary>
			/// Windows has detected the battery on this device is too low to continue.  Windows is shutting down.  Age and frequency of this file being present are
			/// unknown.
			/// </summary>
			/// <seealso cref="windowsBatteryCritical"/>
			windowsBatteryCritical,

			/// <summary>
			/// Windows has detected the battery on this device needs to be charged.  However, it isn't to the point that the computer needs to shut down.
			/// </summary>
			/// <seealso cref="windowsBatteryLow"/>
			windowsBatteryLow,

			/// <summary>
			/// Windows has run into a problem forcing it to restart.
			/// </summary>
			windowsCriticalStop,

			/// <summary>
			/// This is just a default sound.
			/// </summary>
			default_,

			/// <summary>
			/// Should be similar to <see cref="ding"/>, but you probably shouldn't assume this file exists.  Just use <see cref="ding"/> instead.
			/// </summary>
			windowsDing,

			/// <summary>
			/// Standard error sound.  Could be used in situations similar to how <see cref="hand"/> would be used.  However, this is a <see cref="SoundFile"/>
			/// while <see cref="hand"/> represents a <see cref="SysSound"/>.
			/// </summary>
			error,

			/// <summary>
			/// Probably similar to <see cref="exclamation"/>.  However, that's a <see cref="SysSound"/> while this is a <see cref="SoundFile"/>.  Also, <see
			/// cref="exclamation"/> is more likely to be on your target machines.
			/// </summary>
			windowsExclamation,

			/// <summary>
			/// Windows has discovered a new RSS feed.  This probably assumes Internet Explorer (or maybe Edge) is the browser.  However, it might be available on
			/// any machine with those browsers.
			/// </summary>
			windowsFeedDiscovered,

			/// <summary>
			/// This has something to do with the foreground in Windows.  Details are unknown.  Nor is it known how likely you are to find this on some machines.
			/// </summary>
			windowsFg,

			/// <summary>
			/// Windows has detected an unspecified hardware failure.  Don't assume this sound is present.
			/// </summary>
			windowsHardwareFail,

			/// <summary>
			/// Windows has detected a new hardware device.  It was probably added for USB and Firewire support.  However, you shouldn't assume this sound will be
			/// present.
			/// </summary>
			windowsHardwareInsert,

			/// <summary>
			/// Windows has detected a hardware device was removed.  It was probably added for USB and Firewire support.  However, you shouldn't assume this sound
			/// will be present.
			/// </summary>
			windowsHardwareRemove,

			/// <summary>
			/// The info bar was displayed.  This was probably added by Windows 8 and would then be present in any later machine.
			/// </summary>
			windowsInfoBar,

			/// <summary>
			/// The user is logging off.  This should be present on any modern Windows PC.
			/// </summary>
			windowsLogOff,

			/// <summary>
			/// The user is logging on.  This should be present on any modern Windows PC.
			/// </summary>
			windowsLogOn,

			/// <summary>
			/// Details are unknown, but it probably has something to do with the Windows Start Menu.  Don't assume it's present.
			/// </summary>
			windowsMenuCmd,

			/// <summary>
			/// It's unknown what this sound represents.  It's unknown how many PCs have this sound.  It's a <see cref="SoundFile"/>.
			/// </summary>
			windowsMsgNudge,

			/// <summary>
			/// A window was minimized by the user.  You can count on this file existing.
			/// </summary>
			windowsMinimize,

			/// <summary>
			/// This sound was played by some versions of Internet Explorer and Windows Explorer when the current page shown changed.  It should be present on
			/// anything newer than Windows 98 or than has IE on it.
			/// </summary>
			windowsNavStart,

			/// <summary>
			/// Windows is notifying the user that a calendar event has occurred.  Probably only available on machines that received Microsoft's calendar app—so
			/// don't expect it.
			/// </summary>
			windowsNotifyCal,

			/// <summary>
			/// Windows is notifying the user they received new email.
			/// </summary>
			windowsNotifyEmail,

			/// <summary>
			/// Windows is notifying the user they received a message on a messaging system other than email.  It could be SMS, Facebook, Twitter, or anything else
			/// supported.  Don't assume it's present.
			/// </summary>
			windowsNotifyMessaging,

			/// <summary>
			/// Windows is sending the user a generic system notification.  Don't count on this file being present.  It's age is unknown.
			/// </summary>
			windowsNotifySysGeneric,

			/// <summary>
			/// Windows is notifying the user of an unspecified event.  Don't count on this sound existing on your target PC.
			/// </summary>
			windowsNotifyUnspecified,

			/// <summary>
			/// The browser has blocked a pop-up.  Don't assume this file is present.
			/// </summary>
			windowsPopUpBlocked,

			/// <summary>
			/// Windows has completed printing a file.  This file should exist on most Windows installations.
			/// </summary>
			windowsPrintComplete,

			/// <summary>
			/// This has something to do with proximity to a connection to something.  Don't count on this file.
			/// </summary>
			/// <seealso cref="windowsProximityNotification"/>
			windowsProximityConnection,

			/// <summary>
			/// This has something to do with proximity to something.  However, it's unknown what the difference between this and <see
			/// cref="windowsProximityConnection"/> is. Don't count on this file.
			/// </summary>
			windowsProximityNotification,

			/// <summary>
			/// This should be similar to <see cref="recycle"/>.  However, you may not be able to count on this file.
			/// </summary>
			windowsRecycle,

			/// <summary>
			/// A window that was either maximized or minimized has been restored.  While a sound was associated with such an event for a long time, it's unknown if
			/// this is that sound.
			/// </summary>
			windowsRestore,

			/// <summary>
			/// In addition to the 10 phone ringing sounds (<see cref="ring01"/>, <see cref="ring02"/>, <see cref="ring03"/>, <see cref="ring04"/>, <see
			/// cref="ring05"/>, <see cref="ring06"/>, <see cref="ring07"/>, <see cref="ring08"/>, <see cref="ring09"/>, and <see cref="ring10"/>) plus <see
			/// cref="ringOut"/>, we have this sound and <see cref="windowsRingOut"/>.  A "ring in" sound has existed for a long time, but this might or might not
			/// be it.
			/// </summary>
			windowsRingIn,

			/// <summary>
			/// In addition to the 10 phone ringing sounds (<see cref="ring01"/>, <see cref="ring02"/>, <see cref="ring03"/>, <see cref="ring04"/>, <see
			/// cref="ring05"/>, <see cref="ring06"/>, <see cref="ring07"/>, <see cref="ring08"/>, <see cref="ring09"/>, and <see cref="ring10"/>) plus <see
			/// cref="ringOut"/>, we have this sound and <see cref="windowsRingIn"/>.  It's unknown how old this sound is.
			/// </summary>
			windowsRingOut,

			/// <summary>
			/// Windows is shutting down in a normal fashion.  This sound has existed since at least Windows 95.
			/// </summary>
			windowsShutDown,

			/// <summary>
			/// Windows is starting up.  The user hasn't logged in.  This sound has existing since at least Windows 95.
			/// </summary>
			windowsStartUp,

			/// <summary>
			/// The user has unlocked Windows.  It's unknown how old this sound is.
			/// </summary>
			windowsUnlock,

			/// <summary>
			/// Windows has activated the secure desktop so it can ask the user a question without any chance of malware intercepting it.  Installations of Windows
			/// older than Vista probably won't have this file.
			/// </summary>
			windowsUAC,
		}

		/// <summary>
		/// Implementation of <see cref="ISound"/> that handles sounds that Windows .NET declares as <see cref="System.Media.SystemSound"/>.  You will never need to declare an
		/// instance of this class.
		/// </summary>
		public class SysSound : ISound
		{
			#region Constructors
				/// <summary>
				/// Creates a <see cref="SysSound"/> for the specified <see cref="System.Media.SystemSound"/> instance.
				/// </summary>
				/// <param name="sysSound">The .NET sound to wrap</param>
				private SysSound(System.Media.SystemSound sysSound)
					=> this.sysSound = sysSound;
			#endregion

			#region Delegates
			#endregion

			#region Events
			#endregion

			#region Constants
			#endregion

			#region Helper Types
			#endregion

			#region Data Members
				/// <summary>
				/// This is the wrapped sound.
				/// </summary>
				public readonly System.Media.SystemSound sysSound;

				/// <summary>
				/// This is the asterisk sound and corresponds to both <see cref="SoundIDs.asterisk"/> and <see cref="System.Media.SystemSounds.Asterisk"/>.
				/// </summary>
				public static readonly SysSound asterisk = new(System.Media.SystemSounds.Asterisk);

				/// <summary>
				/// This is the standard beep sound.  It corresponds to both <see cref="SoundIDs.beep"/> and <see cref="Media.SystemSounds.Beep"/>.
				/// </summary>
				public static readonly SysSound beep = new(System.Media.SystemSounds.Beep);

				/// <summary>
				/// This is the standard "exclamation" warning sound.  It corresponds to both <see cref="SoundIDs.exclamation"/> and <see cref="System.Media.SystemSounds
				/// .Exclamation"/>.  Note, while <see cref="SoundIDs.windowsExclamation"/> has a similar name, that's a local file represented by <see cref="SoundFile"/>.
				/// </summary>
				public static readonly SysSound exclamation = new(System.Media.SystemSounds.Exclamation);

				/// <summary>
				/// This is the standard "error" hand sound.  It corresponds to both <see cref="SoundIDs.hand"/> and <see cref="Media.SystemSounds.Hand"/>.
				/// </summary>
				public static readonly SysSound hand = new(System.Media.SystemSounds.Hand);

				/// <summary>
				/// This is the standard question sound.  It corresponds to both <see cref="SoundIDs.question"/> and <see cref="Media.SystemSounds.Question"/>.
				/// </summary>
				public static readonly SysSound question = new(System.Media.SystemSounds.Question);
			#endregion

			#region Properties
			#endregion

			#region Methods
				/// <summary>
				/// Starts playback of the specified sound.  The sound is preloaded by Windows and always plays asynchronously.
				/// </summary>
				public void Play()
					=> sysSound.Play();

				/// <summary>
				/// The underlying class doesn't support looping, so <see cref="ISound.PlayLooping"/> is mapped to <see cref="Play"/>.
				/// </summary>
				public void PlayLooping()
					=> sysSound.Play();

				/// <summary>
				/// The underlying class doesn't distinguish between synchronous and asynchronous playback.  So <see cref="ISound.PlaySync"/> is mapped to <see
				/// cref="Play"/>
				/// </summary>
				public void PlaySync()
					=> sysSound.Play();


				/// <summary>
				/// While <see cref="ISound.Stop"/> is defined as stopping playback, the underlying class, <see cref="System.Media.SystemSound"/> doesn't support
				/// this.  So this implementation does nothing.
				/// </summary>
				public void Stop()
				{
				}
			#endregion

			#region Event Handlers
			#endregion
		}
	#endregion

	#region Data Members
		/// <summary>
		/// Stores our map of entries.  It's private to prevent modifications by outside classes.
		/// </summary>
		private static readonly System.Collections.Generic.Dictionary<SoundIDs, ISound> mapIdToSoundObj = new()
		{
			[SoundIDs.asterisk] = SysSound.asterisk,
			[SoundIDs.beep] = SysSound.beep,
			[SoundIDs.exclamation] = SysSound.exclamation,
			[SoundIDs.hand] = SysSound.hand,
			[SoundIDs.question] = SysSound.question,

			[SoundIDs.alarm01] = new SoundFile("alarm01.wav", true),
			[SoundIDs.alarm02] = new SoundFile("alarm02.wav", true),
			[SoundIDs.alarm03] = new SoundFile("alarm03.wav", true),
			[SoundIDs.alarm04] = new SoundFile("alarm04.wav", true),
			[SoundIDs.alarm05] = new SoundFile("alarm05.wav", true),
			[SoundIDs.alarm06] = new SoundFile("alarm06.wav", true),
			[SoundIDs.alarm07] = new SoundFile("alarm07.wav", true),
			[SoundIDs.alarm08] = new SoundFile("alarm08.wav", true),
			[SoundIDs.alarm09] = new SoundFile("alarm09.wav", true),
			[SoundIDs.alarm10] = new SoundFile("alarm10.wav", true),
			[SoundIDs.chimes] = new SoundFile("chimes.wav", true),
			[SoundIDs.chord] = new SoundFile("chord.wav", true),
			[SoundIDs.ding] = new SoundFile("ding.wav", true),
			[SoundIDs.flourish] = new SoundFile("flourish.mid", true),
			[SoundIDs.notify] = new SoundFile("notify.wav", true),
			[SoundIDs.oneStop] = new SoundFile("onestop.mid", true),
			[SoundIDs.recycle] = new SoundFile("recycle.wav", true),
			[SoundIDs.ring01] = new SoundFile("ring01.wav", true),
			[SoundIDs.ring02] = new SoundFile("ring02.wav", true),
			[SoundIDs.ring03] = new SoundFile("ring03.wav", true),
			[SoundIDs.ring04] = new SoundFile("ring04.wav", true),
			[SoundIDs.ring05] = new SoundFile("ring05.wav", true),
			[SoundIDs.ring06] = new SoundFile("ring06.wav", true),
			[SoundIDs.ring07] = new SoundFile("ring07.wav", true),
			[SoundIDs.ring08] = new SoundFile("ring08.wav", true),
			[SoundIDs.ring09] = new SoundFile("ring09.wav", true),
			[SoundIDs.ring10] = new SoundFile("ring10.wav", true),
			[SoundIDs.ringOut] = new SoundFile("ringout.wav", true),
			[SoundIDs.speechDisambig] = new SoundFile("Speech Disambiguation.wav", true),
			[SoundIDs.speechMisrecog] = new SoundFile("Speech Misrecognition.wav", true),
			[SoundIDs.speechOff] = new SoundFile("Speech Off.wav", true),
			[SoundIDs.speechOn] = new SoundFile("Speech On.wav", true),
			[SoundIDs.speechSleep] = new SoundFile("Speech Sleep.wav", true),
			[SoundIDs.tada] = new SoundFile("tada.wav", true),
			[SoundIDs.town] = new SoundFile("town.mid", true),
			[SoundIDs.windowsBkg] = new SoundFile("Windows Background.wav", true),
			[SoundIDs.windowsBalloon] = new SoundFile("Windows Balloon.wav", true),
			[SoundIDs.windowsBatteryCritical] = new SoundFile("Windows Battery Critical.wav", true),
			[SoundIDs.windowsBatteryLow] = new SoundFile("Windows Battery Low.wav", true),
			[SoundIDs.windowsCriticalStop] = new SoundFile("Windows Critical Stop.wav", true),
			[SoundIDs.default_] = new SoundFile("Windows Default.wav", true),
			[SoundIDs.windowsDing] = new SoundFile("Windows Ding.wav", true),
			[SoundIDs.error] = new SoundFile("Windows Error.wav", true),
			[SoundIDs.windowsExclamation] = new SoundFile("Windows Exclamation.wav", true),
			[SoundIDs.windowsFeedDiscovered] = new SoundFile("Windows Feed Discovered.wav", true),
			[SoundIDs.windowsFg] = new SoundFile("Windows Foreground.wav", true),
			[SoundIDs.windowsHardwareFail] = new SoundFile("Windows Hardware Fail.wav", true),
			[SoundIDs.windowsHardwareInsert] = new SoundFile("Windows Hardware Insert.wav", true),
			[SoundIDs.windowsHardwareRemove] = new SoundFile("Windows Hardware Remove.wav", true),
			[SoundIDs.windowsInfoBar] = new SoundFile("Windows Information Bar.wav", true),
			[SoundIDs.windowsLogOff] = new SoundFile("Windows Logoff Sound.wav", true),
			[SoundIDs.windowsLogOn] = new SoundFile("Windows Logon.wav", true),
			[SoundIDs.windowsMenuCmd] = new SoundFile("Windows Menu Command.wav", true),
			[SoundIDs.windowsMsgNudge] = new SoundFile("Windows Message Nudge.wav", true),
			[SoundIDs.windowsMinimize] = new SoundFile("Windows Minimize.wav", true),
			[SoundIDs.windowsNavStart] = new SoundFile("Windows Navigation Start.wav", true),
			[SoundIDs.windowsNotifyCal] = new SoundFile("Windows Notify Calendar.wav", true),
			[SoundIDs.windowsNotifyEmail] = new SoundFile("Windows Notify Email.wav", true),
			[SoundIDs.windowsNotifyMessaging] = new SoundFile("Windows Notify Messaging.wav", true),
			[SoundIDs.windowsNotifySysGeneric] = new SoundFile("Windows Notify System Generic.wav", true),
			[SoundIDs.windowsNotifyUnspecified] = new SoundFile("Windows Notify.wav", true),
			[SoundIDs.windowsPopUpBlocked] = new SoundFile("Windows Pop-up Blocked.wav", true),
			[SoundIDs.windowsPrintComplete] = new SoundFile("Windows Print complete.wav", true),
			[SoundIDs.windowsProximityConnection] = new SoundFile("Windows Proximity Connection.wav", true),
			[SoundIDs.windowsProximityNotification] = new SoundFile("Windows Proximity Notification.wav", true),
			[SoundIDs.windowsRecycle] = new SoundFile("Windows Recycle.wav", true),
			[SoundIDs.windowsRestore] = new SoundFile("Windows Restore.wav", true),
			[SoundIDs.windowsRingIn] = new SoundFile("Windows Ringin.wav", true),
			[SoundIDs.windowsRingOut] = new SoundFile("Windows Ringout.wav", true),
			[SoundIDs.windowsShutDown] = new SoundFile("Windows Shutdown.wav", true),
			[SoundIDs.windowsStartUp] = new SoundFile("Windows Startup.wav", true),
			[SoundIDs.windowsUnlock] = new SoundFile("Windows Unlock.wav", true),
			[SoundIDs.windowsUAC] = new SoundFile("Windows User Account Control.wav", true),
		};
	#endregion

	#region Properties
		/// <summary>
		/// Public means of translating a <see cref="SoundIDs"/> into a <see cref="ISound"/>.  This works for all <see cref="SoundIDs"/> whether they be for a
		/// <see cref="SoundFile"/> or a <see cref="SysSound"/>.
		/// </summary>
		public static System.Collections.Generic.IReadOnlyDictionary<SoundIDs, ISound> AllPredefinedSounds => mapIdToSoundObj;
	#endregion

	#region Methods
	#endregion

	#region Event Handlers
	#endregion
}