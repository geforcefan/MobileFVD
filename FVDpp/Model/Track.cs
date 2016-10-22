using System;
using SQLite;
using GlmNet;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;

namespace FVD.Model
{
	[Table("Tracks")]
	public class Track
	{
		[PrimaryKey, AutoIncrement]
		public int TrackID { get; set; }

		[MaxLength(8), Indexed(Name = "TrackProjectID", Order = 2, Unique = true)]
		public int ProjectID { get; set; }

		[MaxLength(64), Indexed(Name = "TrackProjectID", Order = 2, Unique = true)]
		public String Name { get; set; }

		[MaxLength(64)]
		public String Description { get; set; }

		[Ignore]
		public vec3 startPos { get; set; }

		[Ignore]
		public float startYaw { get; set; }

		[Ignore]
		public float startPitch { get; set; } = 0.0f;

		[Ignore]
		public float heartline { get; set; }

		[Ignore]
		public float friction { get; set; } = 0.03f;

		[Ignore]
		public float resistance { get; set; } = 2e-5f;

		public Boolean DrawTrack { get; set; } = true;

		public Boolean DrawHeartline { get; set; } = false;

		[Ignore]
		public ObservableCollection<Section> sections { get; set; } = new ObservableCollection<Section>();

		[Ignore]
		public MNode anchorNode { get; set; } = new MNode(new vec3(0.0f, 0.0f, 0.0f), new vec3(0.0f, 0.0f, -1.0f), 0.0f, 10.0f, 1.0f, 0.0f);

		[Ignore]
		public mat4 anchorBase { get; set; }

		[Ignore]
		public Action<int> OnUpdateTrack { get; set; }

		[Ignore]
		public Action<Section> OnSectionActivated { get; set; }

		[Ignore]
		public Section activeSection { get; set; } = null;

		public Track()
		{
			Name = "";
			Description = "";
			ProjectID = 0;

			sections.CollectionChanged += onSectionsChanged;

			Init(new vec3(0.0f, 5.0f, 0.0f), 0.0f, 1.1f);
		}

		public void Init(vec3 _startPos, float _startYaw, float _heartline)
		{
			startPos = _startPos;
			startYaw = _startYaw;
			startPitch = 0.0f;
			heartline = _heartline;

			MNode _anchorNode = anchorNode;
			_anchorNode.updateNorm();
			_anchorNode.Energy = 0.5f * anchorNode.Velocity * anchorNode.Velocity + Core.Misc.F_G * anchorNode.getPosHeart(0.9f * heartline).y;
			anchorNode = _anchorNode;
		}

		public void insertSection(SectionType type, int index)
		{
			Console.WriteLine("Inserting a Section of type " + type + " at index " + index);

			MNode startNode;
			Section newSection;

			if (sections.Count > 0)
			{
				if (index == -1)
				{
					startNode = sections.Last().nodes.Last();
				}
				else if (index == 0)
				{
					startNode = anchorNode;
					sections.First().nodes.RemoveAt(0);
				}
				else {
					startNode = sections[index - 1].nodes.Last();

					if (sections.Count > index)
					{
						sections[index].nodes.RemoveAt(0);
					}
				}
			}
			else {
				startNode = anchorNode;
			}

			switch (type)
			{
				case SectionType.Straight:
					newSection = new SectionStraight(this, startNode, 10.0f);
					break;
				case SectionType.Forced:
					newSection = new SectionForce(this, startNode, 1000.0f);
					break;
				default:
					newSection = new SectionStraight(this, startNode, 10.0f);
					break;
			}

			if (index == -1)
			{
				sections.Add(newSection);
			}
			else if (index == 0)
			{
				sections.Insert(index, newSection);

				if (sections.Count > index + 1)
				{
					sections[index + 1].nodes.Insert(0, newSection.nodes.Last());
				}
			}
		}

		public void UpdateTrack(int sectionIndex, int nodeIndex)
		{
			updateAnchorBase();

			if (sectionIndex < 0) sectionIndex = 0;
			if (sections.Count <= sectionIndex)
			{
				if (OnUpdateTrack != null)
					OnUpdateTrack(0);
				
				return;
			}

			int nodeAt = (sections[sectionIndex].type == SectionType.Straight || sections[sectionIndex].type == SectionType.Curved) ? 0 : nodeIndex;
			for (int i = 0; i < sectionIndex; ++i)
			{
				nodeAt += sections[i].nodes.Count - 1;
			}

			int updateFrom = sections[sectionIndex].updateSection(nodeIndex);
			for (int i = sectionIndex + 1; i < sections.Count; i++)
			{
				sections[i].nodes.Insert(0, sections[i - 1].nodes[sections[i - 1].nodes.Count - 1]);
				sections[i].updateSection(0);
			}

			nodeAt = nodeAt > getNumPoints(sections[sectionIndex]) + updateFrom ? getNumPoints(sections[sectionIndex]) + updateFrom : nodeAt;

			if (OnUpdateTrack != null)
				OnUpdateTrack(nodeAt);
		}

		public MNode getPoint(int index)
		{
			int i = 0;
			if (index < 0) index = 0;

			while (sections.Count > i && index > sections[i].nodes.Count - 1)
			{
				index -= sections[i++].nodes.Count - 1;
			}

			if (sections.Count == i)
			{
				if (sections.Count > 0) return sections.Last().nodes.Last();
				else return anchorNode;
			}

			return sections[i].nodes[index];
		}

		public void UpdateTrack(Section fromSection, int nodeIndex)
		{
			int i = 0;
			if (sections.Count == 0) return;
			for (; i < sections.Count; ++i)
			{
				if (sections[i] == fromSection) break;
			}
			UpdateTrack(i, nodeIndex);
		}

		public int getNumPoints(Section until = null)
		{
			int sum = 0;
			for (int i = 0; i < sections.Count; ++i)
			{
				if (until != null && sections[i] == until) return sum;
				sum += sections[i].nodes.Count - 1;
			}
			return sum;
		}

		public void activateSection(Section section)
		{
			activeSection = section;

			if (OnSectionActivated != null)
				OnSectionActivated(section);
		}

		public void removeSection(int index)
		{
			Console.WriteLine("Remove Section " + index);

			if (sections.Count <= index) return;

			if (index == sections.Count - 1)
			{
				sections.RemoveAt(index);
			}
			else
			{
				Console.WriteLine("Is a Between section");

				Console.WriteLine("I want to prepend " + sections[index].nodes.First().Pos + " To Section " + (index + 1) + "(" + sections[index + 1] + ")" );

				sections[index + 1].nodes.Insert(0, sections[index].nodes.First());
				sections.RemoveAt(index);
			}
		}

		public void removeSection(Section fromSection)
		{
			if (sections.Count == 0) return;

			for (int i = 0; i < sections.Count; i++)
			{
				Console.WriteLine("Looking in index " + i + " for section " + fromSection);

				if (sections[i] == fromSection)
				{
					removeSection(i);
					break;
				}
			}
		}

		public int getSectionIndex(Section section)
		{
			for (int i = 0; i < sections.Count; i++)
			{
				if (sections[i] == section)
				{
					return i;
				}
			}

			return -1;
		}

		private void updateAnchorBase()
		{
			anchorBase = glm.translate(mat4.identity(), startPos) * glm.rotate(glm.radians(startYaw - 90.0f), new vec3(0.0f, 1.0f, 0.0f));
		}

		private void onSectionsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Console.WriteLine("Section List Changed (" + e.Action + ")");
			Console.WriteLine("e.OldStartingIndex: " + e.OldStartingIndex);
			Console.WriteLine("e.NewStartingIndex: " + e.NewStartingIndex);

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				UpdateTrack(e.NewStartingIndex, 0);
				activateSection(sections[e.NewStartingIndex]);

				sections[e.NewStartingIndex].OnSectionChanged += (Section sec) =>
				{
					UpdateTrack(sec, 0);
				};
			}

			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				int oldSectionsCount = sections.Count + 1;

				if (e.OldStartingIndex == oldSectionsCount - 1)
				{
					int updateIndex = Math.Max(0, e.OldStartingIndex - 1);

					Console.WriteLine("Is the last section, so update from index " + updateIndex);
					UpdateTrack(updateIndex, 0);

					// TODO: Discuss, DO we really have to activate another section when deleting this one?
					/*if (sections.Count != 0)
					{
						activateSection(sections[e.OldStartingIndex - 1]);
					}
					else {
						activateSection(null);
					}*/
					activateSection(null);
				}
				else {
					UpdateTrack(e.OldStartingIndex, 0);

					// TODO: Discuss, DO we really have to activate another section when deleting this one?
					//activateSection(sections[e.OldStartingIndex]);
					activateSection(null);
				}
			}
		}
	}
}