import { Column, Entity, OneToMany, PrimaryGeneratedColumn, UpdateDateColumn } from 'typeorm';

import { User } from './User';
@Entity({ name: 'joblevels' })
export class JobLevel {
  @PrimaryGeneratedColumn('uuid')
  id!: string;

  @Column({
    type: 'varchar', // Kiểu varchar cho cột level
    length: 20, // Độ dài tối đa của chuỗi là 20 ký tự
    enum: ['Intern', 'Fresher', 'Junior', 'Middle', 'Senior', 'Lead', 'Manager', 'CEO'], // Các giá trị Enum
    default: 'Intern', // Giá trị mặc định là 'Intern'
    nullable: false, // Không cho phép giá trị null
  })
  level_name!: 'Intern' | 'Fresher' | 'Junior' | 'Middle' | 'Senior' | 'Lead' | 'Manager' | 'CEO'; // Định nghĩa kiểu dữ liệu cho `level`

  @Column({ type: 'int', default: 0 })
  min_exp?: number;

  @Column({ type: 'int', default: 1000 })
  max_exp!: Date;

  @UpdateDateColumn()
  create_at!: Date;

  @UpdateDateColumn()
  updated_at!: Date;

  @OneToMany(() => User, (user) => user.level)
  user!: User[];
}
