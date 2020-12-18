using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    enum SkillType
    {
        Damage,
        DamageOverTime,
        Control
    }

    class Player
    {
        public string playerName;

        public int linesCleared;
        public int score;

        public int level;
        public int nextLevelLinesNum;

        public List<Skill> skills = new List<Skill>();


        public Player()
        {
            playerName = "Anonymous";
            linesCleared = 0;
            score = 0;
            level = 1;
            nextLevelLinesNum = level * level * (level + 1) /*+ 5*/;
        }

        public void AddSkill(Skill skill)
        {
            skills.Add(skill);
        }

        public Skill GetSkill(int skill_index)
        {
            return skills[skill_index];
        }

        //public bool UseSkill(Skill skill)
        //{
        //    switch (skill.skillType)
        //    {
        //        case SkillType.Damage:
                    
        //            return true;

        //        case SkillType.Control:

        //            return true;

        //        default:
        //            return false;
        //    }
        //}
    }

    class Skill
    {
        public string skillName;
        public SkillType skillType;
        public ConsoleColor skillColor;

        public int skillEffectLines;

        public int skillLastTime;

        public int skillCoolDown;
        public int skillCoolingDown;

        public static Skill CreateDamageSkill(string name, int effectLines, int coolDown, ConsoleColor color)
        {
            Skill skill = new Skill();

            skill.skillName = name;
            skill.skillType = SkillType.Damage;
            skill.skillEffectLines = effectLines;

            skill.skillCoolDown = skill.skillCoolingDown = coolDown;

            skill.skillColor = color;

            return skill;
        }

        public static Skill CreateDOTSkill(string name, int effectLines, int lastTime, int coolDown, ConsoleColor color)
        {
            Skill skill = new Skill();

            skill.skillName = name;
            skill.skillType = SkillType.DamageOverTime;
            skill.skillEffectLines = effectLines;
            skill.skillLastTime = lastTime;

            skill.skillCoolDown = skill.skillCoolingDown = coolDown;

            skill.skillColor = color;

            return skill;
        }

        public static Skill CreateControlSkill(string name, int lastTime, int coolDown, ConsoleColor color)
        {
            Skill skill = new Skill();

            skill.skillName = name;
            skill.skillType = SkillType.DamageOverTime;
            skill.skillLastTime = lastTime;

            skill.skillCoolDown = skill.skillCoolingDown = coolDown;

            skill.skillColor = color;

            return skill;
        }


    }
}